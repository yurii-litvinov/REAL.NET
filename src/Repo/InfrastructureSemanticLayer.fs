(* Copyright 2017 Yurii Litvinov
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License. *)

namespace Repo.InfrastructureSemanticLayer

open Repo
open Repo.DataLayer

/// Helper functions to work with Infrastructure Metamodel.
module InfrastructureMetamodel =
    /// Returns true if given association is a part of an attribute definition.
    let private isAttributeLink (association: IAssociation) =
        if association.Target.IsNone || not (association.Target.Value :? INode) then
            false
        else
            let target = association.Target.Value :?> INode
            association.TargetName = target.Name 
            || association.TargetName = "kind" 
            || association.TargetName = "stringValue"
            || association.TargetName = "enumElement"

    /// Searches for an Infrastructure Metamodel in current repository.
    let infrastructureMetamodel (repo: IRepo) =
        let models = 
            repo.Models 
            |> Seq.filter (fun m -> m.Name = "InfrastructureMetamodel")
        
        if Seq.isEmpty models then
            raise (MalformedInfrastructureMetamodelException "Infrastructure Metamodel not found in a repository")
        elif Seq.length models <> 1 then
            raise (MalformedInfrastructureMetamodelException 
                    "There is more than one Infrastructure Metamodel in a repository")
        else
            Seq.head models

    let findNode (repo: IRepo) name =
        let metamodel = infrastructureMetamodel repo
        CoreSemanticLayer.Model.findNode metamodel name

    let findAssociation (repo: IRepo) targetName =
        let metamodel = infrastructureMetamodel repo
        // TODO: Looks like kind of hack, but we need to find correct associations here.
        if targetName = "kind" || targetName = "stringValue" then
            let attribute = findNode repo "Attribute"
            CoreSemanticLayer.Model.findAssociationWithSource metamodel attribute targetName
        else
            CoreSemanticLayer.Model.findAssociation metamodel targetName

    let private node (repo: IRepo) = findNode repo "Node"

    let private edge (repo: IRepo) = findNode repo "Edge"

    let private generalization (repo: IRepo) = findNode repo "Generalization"

    let isFromInfrastructureMetamodel repo element =
        CoreSemanticLayer.Element.containingModel repo element = infrastructureMetamodel repo
    
    let isNode repo (element: IElement) =
        if isFromInfrastructureMetamodel repo element then
            element :? INode && CoreSemanticLayer.Element.hasAttribute repo element "isAbstract"
        else
            CoreSemanticLayer.Element.isInstanceOf (node repo) element

    let isEdge repo (element: IElement) =
        if isFromInfrastructureMetamodel repo element then
            element :? IAssociation  && CoreSemanticLayer.Element.hasAttribute repo element "isAbstract"
        else
            CoreSemanticLayer.Element.isInstanceOf (edge repo) element
            || CoreSemanticLayer.Element.isInstanceOf (generalization repo) element

    let isGeneralization repo (element: IElement) =
        if isFromInfrastructureMetamodel repo element then
            element :? IGeneralization
        else
            CoreSemanticLayer.Element.isInstanceOf (generalization repo) element

    let isRelationship (repo: IRepo) element =
        isEdge repo element || isGeneralization repo element

    let isElement repo element =
        isNode repo element || isRelationship repo element

/// Helper module for working with elements in Infrastructure Metamodel terms.
module Element =
    let private thisElementAttributes repo element =
        let rec isAttributeAssociation (a: IAssociation) =
            let attributesAssociation = InfrastructureMetamodel.findAssociation repo "attributes"
            if CoreSemanticLayer.Element.isInstanceOf attributesAssociation a then
                true
            else
                let target = a.Target
                match target with
                | Some n ->
                    match n with
                    // NOTE: Heuristic to tell attribute associations from other associations in an infrastructure metamodel:
                    // attribute associations can not be an instance of "attributes" association since they are on the same 
                    // metalevel and need to be instances of Language Metamodel associations (or else we break instantiation
                    // chain from Core Metamodel). So we use hidden knowledge about such associations.
                    | :? INode as n when InfrastructureMetamodel.isFromInfrastructureMetamodel repo n ->
                        a.TargetName = n.Name
                    | _ -> match a.Class with
                           | :? IAssociation -> isAttributeAssociation (a.Class :?> IAssociation)
                           | _ -> false
                | _ -> false

        element
        |> CoreSemanticLayer.Element.outgoingAssociations repo
        |> Seq.filter isAttributeAssociation
        |> Seq.map (fun l -> l.Target)
        |> Seq.choose id
        |> Seq.filter (fun e -> e :? DataLayer.INode)
        |> Seq.cast<DataLayer.INode>

    /// Returns a list of all attributes for an element, respecting generalization. Attributes of most special node
    /// are the first in resulting sequence.
    let attributes repo element =
        let parentsAttributes =
            CoreSemanticLayer.Element.parents repo element 
            |> Seq.map (thisElementAttributes repo) 
            |> Seq.concat 

        Seq.append (thisElementAttributes repo element) parentsAttributes

    /// Returns true if an attribute with given name is present in given element.
    let hasAttribute repo element name =
        attributes repo element |> Seq.filter (fun attr -> attr.Name = name) |> Seq.isEmpty |> not
        
    /// Returns value of an attribute with given name.
    let attributeValue repo element attributeName =
        let values =
            attributes repo element
            |> Seq.filter (fun attr -> attr.Name = attributeName)
            |> Seq.map (fun attr -> CoreSemanticLayer.Element.attributeValue repo attr "stringValue")

        if Seq.isEmpty values then
            raise (InvalidSemanticOperationException <| sprintf "Attribute %s not found" attributeName)
        else
            // TODO: Check that it is actually last overriden attribute, not some attribute from one of the parents.
            // TODO: Also do something with correctness of attribute inheritance.
            Seq.head values

    /// Sets value for a given attribute to a given value.
    let setAttributeValue repo element attributeName value =
        let attributes = 
            thisElementAttributes repo element
            |> Seq.filter (fun attr -> attr.Name = attributeName)
     
        if Seq.isEmpty attributes then
            raise (InvalidSemanticOperationException <| sprintf "Attribute %s not found" attributeName)
        else
            // TODO: Check that it is actually last overriden attribute, not some attribute from one of the parents.
            // TODO: Also do something with correctness of attribute inheritance.
            CoreSemanticLayer.Element.setAttributeValue repo (Seq.head attributes) "stringValue" value

    /// Adds a new attribute to a given element.
    let addAttribute repo element name kind =
        let model = CoreSemanticLayer.Element.containingModel repo element
        let infrastructureModel = InfrastructureMetamodel.infrastructureMetamodel repo
        let attributeNode = CoreSemanticLayer.Model.findNode infrastructureModel "Attribute"
        let attributeKindNode = CoreSemanticLayer.Model.findNode infrastructureModel "AttributeKind"
        let stringNode = CoreSemanticLayer.Model.findNode infrastructureModel "String"

        let kindAssociation = CoreSemanticLayer.Model.findAssociationWithSource infrastructureModel attributeNode "kind"
        let stringValueAssociation = CoreSemanticLayer.Model.findAssociationWithSource infrastructureModel attributeNode "stringValue"
        let attributesAssociation = CoreSemanticLayer.Model.findAssociation infrastructureModel "attributes"

        let attribute = model.CreateNode(name, attributeNode)

        CoreSemanticLayer.Element.addAttribute repo attribute "kind" attributeKindNode kindAssociation kind
        CoreSemanticLayer.Element.addAttribute repo attribute "stringValue" stringValueAssociation stringNode ""
        model.CreateAssociation(attributesAssociation, element, attribute, name) |> ignore

        ()

/// Module containing semantic operations on elements.
module Operations =
    /// Returns link corresponding to an attribute respecting generalization hierarchy.
    let rec private attributeLink repo element attribute =
        let myAttributeLinks e = 
            CoreSemanticLayer.Element.outgoingAssociations repo e
            |> Seq.filter (fun a -> a.Target = Some attribute)

        let attributeLinks = 
            CoreSemanticLayer.Element.parents repo element |> Seq.map myAttributeLinks |> Seq.concat |> Seq.append (myAttributeLinks element) 

        if Seq.isEmpty attributeLinks then
            raise (InvalidSemanticOperationException "Attribute link not found for attribute")
        elif Seq.length attributeLinks <> 1 then
            raise (InvalidSemanticOperationException "Attribute connected with multiple links to a node, model error.")
        else
            Seq.head attributeLinks

    let private copySimpleAttribute repo element (``class``: IElement) name =
        let attributeClassNode = CoreSemanticLayer.Element.attribute repo ``class`` name
        let attributeAssociation = InfrastructureMetamodel.findAssociation repo name
        let defaultValue = CoreSemanticLayer.Element.attributeValue repo ``class`` name
        CoreSemanticLayer.Element.addAttribute repo element name attributeClassNode attributeAssociation defaultValue

    let private addAttribute repo (element: IElement) (attributeClass: INode) =
        let model = CoreSemanticLayer.Element.containingModel repo element
        if Element.hasAttribute repo element attributeClass.Name then
            let valueFromClass = CoreSemanticLayer.Element.attributeValue repo attributeClass "stringValue"
            Element.setAttributeValue repo element attributeClass.Name valueFromClass
        else 
            let attributeLink = attributeLink repo element.Class attributeClass
            let attributeNode = model.CreateNode(attributeClass.Name, attributeClass)
            model.CreateAssociation(attributeLink, element, attributeNode, attributeClass.Name) |> ignore

            copySimpleAttribute repo attributeNode attributeClass "stringValue"
            copySimpleAttribute repo attributeNode attributeClass "kind"

    let instantiate (repo: IRepo) (model: IModel) (``class``: IElement) =
        if Element.attributeValue repo ``class`` "isAbstract" <> "false" then
            raise (InvalidSemanticOperationException "Trying to instantiate abstract node")
        
        let name = 
            match ``class`` with
            | :? INode as n -> "a" + n.Name
            | :? IAssociation as a -> a.TargetName
            | _ -> raise (InvalidSemanticOperationException 
                    "Trying to instantiate something that should not be instantiated")

        let newElement = 
            if Element.attributeValue repo ``class`` "instanceMetatype" = "Metatype.Node" then
                model.CreateNode(name, ``class``) :> IElement
            else
                model.CreateAssociation(``class``, None, None, name) :> IElement

        let attributes = Element.attributes repo ``class``
        attributes |> Seq.rev |> Seq.iter (addAttribute repo newElement)

        newElement
