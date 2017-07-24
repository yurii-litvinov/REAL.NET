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

    let element = findNode "Element"
    let node = findNode "Node"
    let edge = findNode "Edge"
    let association = findNode "Association"
    let generalization = findNode "Generalization"
    let attribute = findNode "Attribute"
    let attributeKind = findNode "AttributeKind"
    let stringNode = findNode "String"

    let attributesAssociation = CoreSemanticLayer.Model.findAssociationWithSource element "attributes"
    let attributeKindAssociation = CoreSemanticLayer.Model.findAssociationWithSource attribute "kind"
    let attributeStringValueAssociation = CoreSemanticLayer.Model.findAssociationWithSource attribute "stringValue"

    member this.Model = infrastructureMetamodel

    member this.Node = node

    member this.Association = association

    member this.Generalization = generalization

    member this.Attribute = attribute
    member this.AttributeKind = attributeKind
    member this.String = stringNode

    member this.AttributesAssociation = attributesAssociation
    member this.AttributeKindAssociation = attributeKindAssociation
    member this.AttributeStringValueAssociation = attributeStringValueAssociation

    member this.IsFromInfrastructureMetamodel element =
        CoreSemanticLayer.Element.containingModel element = infrastructureMetamodel

    member this.IsNode (element: IElement) =
        if this.IsFromInfrastructureMetamodel element then
            // Kind of hack, here we know that all nodes supposed to be instantiated have linguistic attributes, like
            // "isAbstract". Actually, they are instances of "Node supposed to be instantiated" class, so it shall
            // be stated explicitly in metamodel hierarchy.
            element :? INode && CoreSemanticLayer.Element.hasAttribute element "isAbstract"
        else
            CoreSemanticLayer.Element.isInstanceOf this.Node element

    member this.IsAssociation (element: IElement) =
        if this.IsFromInfrastructureMetamodel element then
            element :? IAssociation  && CoreSemanticLayer.Element.hasAttribute element "isAbstract"
        else
            CoreSemanticLayer.Element.isInstanceOf edge element

    member this.IsGeneralization (element: IElement) =
        if this.IsFromInfrastructureMetamodel element then
            element :? IGeneralization
        else
            CoreSemanticLayer.Element.isInstanceOf this.Generalization element

    member this.IsEdge element =
        this.IsAssociation element || this.IsGeneralization element

    member this.IsElement element =
        this.IsNode element || this.IsEdge element

/// Helper for working with elements in Infrastructure Metamodel terms.
type ElementHelper(infrastructureMetamodel: InfrastructureMetamodel) =
    /// Returns attributes of only this element, ignoring generalizations.
    let thisElementAttributes element =
        let rec isAttributeAssociation (a: IAssociation) =
            let attributesAssociation = infrastructureMetamodel.AttributesAssociation
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

    /// Returns true if this element has given attribute ignoring generalization hierarchy.
    let thisElementHasAttribute element name =
        thisElementAttributes element |> Seq.filter (fun attr -> attr.Name = name) |> Seq.isEmpty |> not

    /// Returns underlying Infrastructure Metamodel.
    member this.InfrastructureMetamodel = infrastructureMetamodel

    /// Returns a list of all attributes for an element, respecting generalization. Attributes of most special node
    /// are the first in resulting sequence.
    /// TODO: Actually do BFS and ignore overriden attributes.
    member this.Attributes element =
        let parentsAttributes =
            CoreSemanticLayer.Element.parents element
            |> Seq.map thisElementAttributes
            |> Seq.concat

        Seq.append (thisElementAttributes element) parentsAttributes

    /// Returns true if an attribute with given name is present in given element.
    /// TODO: Actually do BFS and stop on first matching attribute.
    member this.HasAttribute element name =
        this.Attributes element |> Seq.filter (fun attr -> attr.Name = name) |> Seq.isEmpty |> not

    /// Returns value of an attribute with given name.
    /// TODO: Actually do BFS and stop on first matching attribute.
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

    /// Adds a new attribute to a given element and initializes it with given value.
    member this.AddAttribute element name kind value =
        if thisElementHasAttribute element name then
            raise (InvalidSemanticOperationException <| sprintf "Attribute %s already present" name)
        let model = CoreSemanticLayer.Element.containingModel element
        let infrastructureModel = infrastructureMetamodel.Model
        let attributeNode = infrastructureMetamodel.Attribute
        let attributeKindNode = infrastructureMetamodel.AttributeKind
        let stringNode = infrastructureMetamodel.String

        let kindAssociation = infrastructureMetamodel.AttributeKindAssociation
        let stringValueAssociation = infrastructureMetamodel.AttributeStringValueAssociation
        let attributesAssociation = infrastructureMetamodel.AttributesAssociation

        let attribute = model.CreateNode(name, attributeNode)

        CoreSemanticLayer.Element.addAttribute attribute "kind" attributeKindNode kindAssociation kind
        CoreSemanticLayer.Element.addAttribute attribute "stringValue" stringValueAssociation stringNode value
        model.CreateAssociation(attributesAssociation, element, attribute, name) |> ignore

        ()

    /// Sets value for a given attribute to a given value. Copies it from parent if needed.
    member this.SetAttributeValue element attributeName value =
        let attribute = thisElementAttributes element|> Seq.tryFind (fun attr -> attr.Name = attributeName)
        if attribute.IsSome then
            CoreSemanticLayer.Element.setAttributeValue attribute.Value "stringValue" value
        else
            let parentsAttributes =
                CoreSemanticLayer.Element.parents element
                |> Seq.map thisElementAttributes
                |> Seq.concat

            let parentAttribute = parentsAttributes |> Seq.tryFind (fun attr -> attr.Name = attributeName)
            if parentAttribute.IsNone then
                raise (AttributeNotFoundException attributeName)
            let parentAttribute = parentAttribute.Value
            let parentAttributeKind = CoreSemanticLayer.Element.attributeValue parentAttribute "kind"
            this.AddAttribute element attributeName parentAttributeKind value

/// Module containing semantic operations on elements.
module private Operations =
    /// Returns link corresponding to an attribute respecting generalization hierarchy.
    let rec private attributeLink element attribute =
        let myAttributeLinks e =
            CoreSemanticLayer.Element.outgoingAssociations e
            |> Seq.filter (fun a -> a.Target = Some attribute)

        let attributeLinks =
            CoreSemanticLayer.Element.parents element
            |> Seq.map myAttributeLinks
            |> Seq.concat
            |> Seq.append (myAttributeLinks element)

        if Seq.isEmpty attributeLinks then
            raise (InvalidSemanticOperationException "Attribute link not found for attribute")
        elif Seq.length attributeLinks <> 1 then
            raise (InvalidSemanticOperationException "Attribute connected with multiple links to a node, model error.")
        else
            Seq.head attributeLinks

    let private copySimpleAttribute (elementHelper: ElementHelper) element (``class``: IElement) name =
        let attributeClassNode = CoreSemanticLayer.Element.attribute ``class`` name
        let attributeAssociation =
            match name with
            | "kind" -> elementHelper.InfrastructureMetamodel.AttributeKindAssociation
            | "stringValue" -> elementHelper.InfrastructureMetamodel.AttributeStringValueAssociation
            | _ -> failwith "Unknown simple attribute name"

        let defaultValue = CoreSemanticLayer.Element.attributeValue ``class`` name
        CoreSemanticLayer.Element.addAttribute element name attributeClassNode attributeAssociation defaultValue

    let private addAttribute (element: IElement) (elementHelper: ElementHelper) (attributeClass: INode)  =
        let model = CoreSemanticLayer.Element.containingModel element
        if elementHelper.HasAttribute element attributeClass.Name then
            let valueFromClass = CoreSemanticLayer.Element.attributeValue attributeClass "stringValue"
            elementHelper.SetAttributeValue element attributeClass.Name valueFromClass
        else
            let attributeLink = attributeLink element.Class attributeClass
            let attributeNode = model.CreateNode(attributeClass.Name, attributeClass)
            model.CreateAssociation(attributeLink, element, attributeNode, attributeClass.Name) |> ignore

            copySimpleAttribute elementHelper attributeNode attributeClass "stringValue"
            copySimpleAttribute elementHelper attributeNode attributeClass "kind"

    let instantiate (elementHelper: ElementHelper) (model: IModel) (``class``: IElement) =
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
        attributes |> Seq.rev |> Seq.iter (addAttribute newElement elementHelper)

        newElement

type InfrastructureSemantic(repo: IRepo) =
    let infrastructureMetamodel = InfrastructureMetamodel(repo)
    let elementHelper = ElementHelper(infrastructureMetamodel)

    member this.Instantiate (model: IModel) (``class``: IElement) =
        Operations.instantiate elementHelper model ``class``

    member this.Metamodel = infrastructureMetamodel
    member this.Element = elementHelper
