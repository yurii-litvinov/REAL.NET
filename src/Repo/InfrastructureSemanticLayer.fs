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

/// Helper for working with Infrastructure Metamodel.
type InfrastructureMetamodel(repo: IRepo) =
    /// Returns true if given association is a part of an attribute definition.
    let isAttributeLink (association: IAssociation) =
        if association.Target.IsNone || not (association.Target.Value :? INode) then
            false
        else
            let target = association.Target.Value :?> INode
            association.TargetName = target.Name 
            // TODO: ???
            || association.TargetName = "kind" 
            || association.TargetName = "stringValue"
            || association.TargetName = "enumElement"

    let infrastructureMetamodel =
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

    let findNode name = CoreSemanticLayer.Model.findNode infrastructureMetamodel name

    let node = findNode "Node"
    let association = findNode "Association"
    let generalization = findNode "Generalization"
    let attribute = findNode "Attribute"

    member this.InfrastructureMetamodel = infrastructureMetamodel

    member this.FindNode name = findNode name

    member this.FindAssociation targetName =
        // TODO: Looks like kind of hack, but we need to find correct associations here.
        if targetName = "kind" || targetName = "stringValue" then
            CoreSemanticLayer.Model.findAssociationWithSource attribute targetName
        else
            CoreSemanticLayer.Model.findAssociation infrastructureMetamodel targetName

    member this.Node = node

    member this.Association (repo: IRepo) = association

    member this.Generalization (repo: IRepo) = generalization

    member this.IsFromInfrastructureMetamodel element =
        CoreSemanticLayer.Element.containingModel element = infrastructureMetamodel
    
    member this.IsNode (element: IElement) =
        if this.IsFromInfrastructureMetamodel element then
            element :? INode && CoreSemanticLayer.Element.hasAttribute element "isAbstract"
        else
            CoreSemanticLayer.Element.isInstanceOf this.Node element

    member this.IsAssociation (element: IElement) =
        if this.IsFromInfrastructureMetamodel element then
            element :? IAssociation  && CoreSemanticLayer.Element.hasAttribute element "isAbstract"
        else
            CoreSemanticLayer.Element.isInstanceOf (this.Association repo) element
            || CoreSemanticLayer.Element.isInstanceOf (this.Generalization repo) element

    member this.IsGeneralization (element: IElement) =
        if this.IsFromInfrastructureMetamodel element then
            element :? IGeneralization
        else
            CoreSemanticLayer.Element.isInstanceOf (this.Generalization repo) element

    member this.IsEdge element =
        this.IsAssociation element || this.IsGeneralization element

    member this.IsElement element =
        this.IsNode element || this.IsEdge element

/// Helper for working with elements in Infrastructure Metamodel terms.
type ElementHelper(repo: IRepo) =
    let infrastructureMetamodel = InfrastructureMetamodel(repo)

    /// Returns attributes of only this element, ignoring generalizations.
    let thisElementAttributes element =
        let rec isAttributeAssociation (a: IAssociation) =
            let attributesAssociation = infrastructureMetamodel.FindAssociation "attributes"
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
                    | :? INode as n when infrastructureMetamodel.IsFromInfrastructureMetamodel n ->
                        a.TargetName = n.Name
                    | _ -> match a.Class with
                           | :? IAssociation -> isAttributeAssociation (a.Class :?> IAssociation)
                           | _ -> false
                | _ -> false

        element
        |> CoreSemanticLayer.Element.outgoingAssociations
        |> Seq.filter isAttributeAssociation
        |> Seq.map (fun l -> l.Target)
        |> Seq.choose id
        |> Seq.filter (fun e -> e :? DataLayer.INode)
        |> Seq.cast<DataLayer.INode>

    member this.InfrastructureMetamodel = infrastructureMetamodel

    /// Returns a list of all attributes for an element, respecting generalization. Attributes of most special node
    /// are the first in resulting sequence.
    member this.Attributes element =
        let parentsAttributes =
            CoreSemanticLayer.Element.parents element 
            |> Seq.map thisElementAttributes 
            |> Seq.concat 

        Seq.append (thisElementAttributes element) parentsAttributes

    /// Returns true if an attribute with given name is present in given element.
    member this.HasAttribute element name =
        this.Attributes element |> Seq.filter (fun attr -> attr.Name = name) |> Seq.isEmpty |> not
        
    /// Returns value of an attribute with given name.
    member this.AttributeValue element attributeName =
        let values =
            this.Attributes element
            |> Seq.filter (fun attr -> attr.Name = attributeName)
            |> Seq.map (fun attr -> CoreSemanticLayer.Element.attributeValue attr "stringValue")

        if Seq.isEmpty values then
            raise (AttributeNotFoundException attributeName)
        else
            // TODO: Check that it is actually last overriden attribute, not some attribute from one of the parents.
            // TODO: Also do something with correctness of attribute inheritance.
            Seq.head values

    /// Sets value for a given attribute to a given value.
    member this.SetAttributeValue element attributeName value =
        let attributes = 
            thisElementAttributes element
            |> Seq.filter (fun attr -> attr.Name = attributeName)
     
        if Seq.isEmpty attributes then
            raise (AttributeNotFoundException attributeName)
        else
            // TODO: Check that it is actually last overriden attribute, not some attribute from one of the parents.
            // TODO: Also do something with correctness of attribute inheritance.
            CoreSemanticLayer.Element.setAttributeValue (Seq.head attributes) "stringValue" value

    /// Adds a new attribute to a given element.
    member this.AddAttribute element name kind =
        let model = CoreSemanticLayer.Element.containingModel element
        let infrastructureModel = infrastructureMetamodel.InfrastructureMetamodel
        let attributeNode = CoreSemanticLayer.Model.findNode infrastructureModel "Attribute"
        let attributeKindNode = CoreSemanticLayer.Model.findNode infrastructureModel "AttributeKind"
        let stringNode = CoreSemanticLayer.Model.findNode infrastructureModel "String"

        let kindAssociation = CoreSemanticLayer.Model.findAssociationWithSource attributeNode "kind"
        let stringValueAssociation = CoreSemanticLayer.Model.findAssociationWithSource attributeNode "stringValue"
        let attributesAssociation = CoreSemanticLayer.Model.findAssociation infrastructureModel "attributes"

        let attribute = model.CreateNode(name, attributeNode)

        CoreSemanticLayer.Element.addAttribute attribute "kind" attributeKindNode kindAssociation kind
        CoreSemanticLayer.Element.addAttribute attribute "stringValue" stringValueAssociation stringNode ""
        model.CreateAssociation(attributesAssociation, element, attribute, name) |> ignore

        ()

/// Module containing semantic operations on elements.
module Operations =
    /// Returns link corresponding to an attribute respecting generalization hierarchy.
    let rec private attributeLink repo element attribute =
        let myAttributeLinks e = 
            CoreSemanticLayer.Element.outgoingAssociations e
            |> Seq.filter (fun a -> a.Target = Some attribute)

        let attributeLinks = 
            CoreSemanticLayer.Element.parents element |> Seq.map myAttributeLinks |> Seq.concat |> Seq.append (myAttributeLinks element) 

        if Seq.isEmpty attributeLinks then
            raise (InvalidSemanticOperationException "Attribute link not found for attribute")
        elif Seq.length attributeLinks <> 1 then
            raise (InvalidSemanticOperationException "Attribute connected with multiple links to a node, model error.")
        else
            Seq.head attributeLinks

    let private copySimpleAttribute (elementHelper: ElementHelper) element (``class``: IElement) name =
        let attributeClassNode = CoreSemanticLayer.Element.attribute ``class`` name
        let attributeAssociation = elementHelper.InfrastructureMetamodel.FindAssociation name
        let defaultValue = CoreSemanticLayer.Element.attributeValue ``class`` name
        CoreSemanticLayer.Element.addAttribute element name attributeClassNode attributeAssociation defaultValue

    let private addAttribute repo (element: IElement) (elementHelper: ElementHelper) (attributeClass: INode)  =
        let model = CoreSemanticLayer.Element.containingModel element
        if elementHelper.HasAttribute element attributeClass.Name then
            let valueFromClass = CoreSemanticLayer.Element.attributeValue attributeClass "stringValue"
            elementHelper.SetAttributeValue element attributeClass.Name valueFromClass
        else 
            let attributeLink = attributeLink repo element.Class attributeClass
            let attributeNode = model.CreateNode(attributeClass.Name, attributeClass)
            model.CreateAssociation(attributeLink, element, attributeNode, attributeClass.Name) |> ignore

            copySimpleAttribute elementHelper attributeNode attributeClass "stringValue"
            copySimpleAttribute elementHelper attributeNode attributeClass "kind"

    let instantiate (repo: IRepo) (model: IModel) (``class``: IElement) =
        let elementHelper = ElementHelper(repo)
        if elementHelper.AttributeValue ``class`` "isAbstract" <> "false" then
            raise (InvalidSemanticOperationException "Trying to instantiate abstract node")
        
        let name = 
            match ``class`` with
            | :? INode as n -> "a" + n.Name
            | :? IAssociation as a -> a.TargetName
            | _ -> raise (InvalidSemanticOperationException 
                    "Trying to instantiate something that should not be instantiated")

        let newElement = 
            if elementHelper.AttributeValue ``class`` "instanceMetatype" = "Metatype.Node" then
                model.CreateNode(name, ``class``) :> IElement
            else
                model.CreateAssociation(``class``, None, None, name) :> IElement

        let attributes = elementHelper.Attributes ``class``
        attributes |> Seq.rev |> Seq.iter (addAttribute repo newElement elementHelper)

        newElement
