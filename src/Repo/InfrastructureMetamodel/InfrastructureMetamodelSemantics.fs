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

namespace Repo.InfrastructureMetamodel

open Repo
open Repo.DataLayer

/// Helper for working with Infrastructure Metamodel.
type InfrastructureMetamodel(repo: IDataRepository) =
    let infrastructureMetamodel = repo.Model "InfrastructureMetamodel"
    let elementSemantics = Repo.AttributeMetamodel.ElementSemantics(repo)
    
    let findNode name = infrastructureMetamodel.Node name

    let element = findNode "Element"
    let node = findNode "Node"
    let edge = findNode "Edge"
    let association = findNode "Association"
    let generalization = findNode "Generalization"
    let attribute = findNode "Attribute"
    let attributeKind = findNode "AttributeKind"
    let stringNode = findNode "String"
    let booleanNode = findNode "Boolean"

    let attributesAssociation = AttributeMetamodel.ModelSemantics.FindAssociationWithSource element "attributes"
    let attributeKindAssociation = AttributeMetamodel.ModelSemantics.FindAssociationWithSource attribute "kind"
    let attributeStringValueAssociation = AttributeMetamodel.ModelSemantics.FindAssociationWithSource attribute "stringValue"
    let attributeIsInstantiableAssociation =
        AttributeMetamodel.ModelSemantics.FindAssociationWithSource attribute "isInstantiable"

    member this.Model = infrastructureMetamodel

    member this.Node = node

    member this.Association = association

    member this.Generalization = generalization

    member this.Attribute = attribute
    member this.AttributeKind = attributeKind
    member this.String = stringNode
    member this.Boolean = booleanNode

    member this.AttributesAssociation = attributesAssociation

    member this.AttributeKindAssociation = attributeKindAssociation
    member this.AttributeStringValueAssociation = attributeStringValueAssociation
    member this.AttributeIsInstantiableAssociation = attributeIsInstantiableAssociation

    member this.IsFromInfrastructureMetamodel element =
        AttributeMetamodel.ElementSemantics.ContainingModel element = infrastructureMetamodel

    member this.IsNode (element: IDataElement) =
        element.LinguisticType = (this.Node :> IDataElement)

    member this.IsAssociation (element: IDataElement) =
        element.LinguisticType = (this.Association :> IDataElement)

    member this.IsGeneralization (element: IDataElement) =
        element.LinguisticType = (this.Generalization :> IDataElement)

    member this.IsEdge element =
        this.IsAssociation element || this.IsGeneralization element

    member this.IsElement element =
        this.IsNode element || this.IsEdge element

/// Helper for working with elements in Infrastructure Metamodel terms.
type ElementHelper(infrastructureMetamodel: InfrastructureMetamodel, repo: IDataRepository) =
    let elementSemantics = Repo.AttributeMetamodel.ElementSemantics repo

(*
    /// Returns attributes of only this element, ignoring generalizations.
    let thisElementAttributes element =
        let rec isAttributeAssociation (a: IDataAssociation) =
            let attributesAssociation = infrastructureMetamodel.AttributesAssociation
            if elementSemantics.IsInstanceOf attributesAssociation a then
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
                    | :? IDataNode as n when infrastructureMetamodel.IsFromInfrastructureMetamodel n ->
                        a.TargetName = n.Name
                    | _ -> match a.Class with
                           | :? IDataAssociation -> isAttributeAssociation (a.Class :?> IDataAssociation)
                           | _ -> false
                | _ -> false

        element
        |> AttributeMetamodel.ElementSemantics.OutgoingAssociations
        |> Seq.filter isAttributeAssociation
        |> Seq.map (fun l -> l.Target)
        |> Seq.choose id
        |> Seq.filter (fun e -> e :? DataLayer.IDataNode)
        |> Seq.cast<DataLayer.IDataNode>

    /// Returns true if this element has given attribute ignoring generalization hierarchy.
    let thisElementHasAttribute element name =
        thisElementAttributes element |> Seq.filter (fun attr -> attr.Name = name) |> Seq.isEmpty |> not

    /// Returns node of an attribute if it is an attribute of this element (ignoring generalization hierarchy) or None
    /// if not.
    let thisElementAttribute element name =
        thisElementAttributes element |> Seq.tryFind (fun attr -> attr.Name = name)

    /// Returns a sequence of all attributes from all parents of this element (not including element itself).
    let parentsAttributes element =
        AttributeMetamodel.ElementSemantics.Parents element
        |> Seq.map thisElementAttributes
        |> Seq.concat

    /// Returns all attributes of this element, including attributes of parents in generalization hierarchy.
    let allAttributes element =
        Seq.append (thisElementAttributes element) (parentsAttributes element) |> Seq.distinctBy (fun attr -> attr.Name)

    /// Searches and returns node of an attribute (with respect to generalization hierarchy). Last overload of an
    /// attribute is returned.
    let attributeNode element name =
        let results = allAttributes element |> Seq.filter (fun attr -> attr.Name = name)
        if Seq.isEmpty results then
            raise (AttributeNotFoundException name)
        else
            Seq.head results

    /// Adds an attribute with given properties to given element.
    let addAttribute element name kind value isInstantiable =
        // TODO: Check correctness of attribute overloading here.
        if thisElementHasAttribute element name then
            raise (InvalidSemanticOperationException <| sprintf "Attribute %s already present" name)
        let model = AttributeMetamodel.ElementSemantics.ContainingModel element
        let infrastructureModel = infrastructureMetamodel.Model
        let attributeNode = infrastructureMetamodel.Attribute
        let attributeKindNode = infrastructureMetamodel.AttributeKind
        let stringNode = infrastructureMetamodel.String
        let booleanNode = infrastructureMetamodel.Boolean

        let kindAssociation = infrastructureMetamodel.AttributeKindAssociation
        let stringValueAssociation = infrastructureMetamodel.AttributeStringValueAssociation
        let isInstantiableAssociation = infrastructureMetamodel.AttributeIsInstantiableAssociation
        let attributesAssociation = infrastructureMetamodel.AttributesAssociation

        let attribute = model.CreateNode(name, attributeNode)

        AttributeMetamodel.ElementSemantics.AddAttribute attribute "kind" attributeKindNode kindAssociation kind
        AttributeMetamodel.ElementSemantics.AddAttribute attribute "stringValue" stringValueAssociation stringNode value
        AttributeMetamodel.ElementSemantics.AddAttribute
            attribute "isInstantiable" isInstantiableAssociation booleanNode isInstantiable

        model.CreateAssociation(attributesAssociation, element, attribute, name) |> ignore

        attribute

    /// Returns attribute node for current element. It is already existing node if this attribute is already an
    /// attribute of an element, or a copy of parent attribute ifit was an attribute of a parent.
    /// Supposed to be used in copy-on-write operations.
    let copyIfNeeded element name =
        let attribute = thisElementAttribute element name
        if attribute.IsSome then
            attribute.Value
        else
            // TODO: Do BFS.
            let parentAttribute = parentsAttributes element |> Seq.tryFind (fun attr -> attr.Name = name)
            if parentAttribute.IsNone then
                raise (AttributeNotFoundException name)
            let parentAttribute = parentAttribute.Value

            let parentAttributeValue = elementSemantics.AttributeValue parentAttribute "stringValue"
            let parentAttributeKind = elementSemantics.AttributeValue parentAttribute "kind"
            let parentAttributeIsInstantiable =
                elementSemantics.AttributeValue parentAttribute "isInstantiable"

            addAttribute element name parentAttributeKind parentAttributeValue parentAttributeIsInstantiable
*)
    /// Returns underlying Infrastructure Metamodel.
    member this.InfrastructureMetamodel = infrastructureMetamodel

    /// Returns a list of all attributes for an element, respecting generalization. Attributes of most special node
    /// are the first in resulting sequence.
    member this.Attributes element = elementSemantics.Attributes element

    /// Returns true if an attribute with given name is present in given element.
    member this.HasAttribute element name = elementSemantics.HasAttribute element name

(*
    /// Adds a new attribute to a given element and initializes it with given value.
    member this.AddAttribute element name kind value =
        addAttribute element name kind value "true" |> ignore

    /// Returns value of an attribute with given name.
    // TODO: Actually do BFS and stop on first matching attribute.
    member this.AttributeValue element attributeName =
        // TODO: Check that it is actually last overriden attribute, not some attribute from one of the parents.
        // TODO: Also do something with correctness of attribute inheritance.
        attributeNode element attributeName
        |> fun attr -> elementSemantics.AttributeValue attr "stringValue"

    /// Sets value for a given attribute to a given value. Copies it from parent if needed.
    member this.SetAttributeValue element attributeName value =
        let attribute = copyIfNeeded element attributeName
        elementSemantics.SetAttributeValue attribute "stringValue" value

    /// Returns true if an attribute is instantiable (passes to the instances), false if instances shall not have it.
    member this.IsAttributeInstantiable element name =
        let attributeNode = attributeNode element name
        elementSemantics.AttributeValue attributeNode "isInstantiable" = "true"

    /// Sets this attribute to instantiable or non-instantiable.
    member this.SetAttributeInstantiable element attributeName (value: bool) =
        let boolToString v =
            match v with
            | true -> "true"
            | false -> "false"

        let attribute = copyIfNeeded element attributeName
        elementSemantics.SetAttributeValue attribute "isInstantiable" (boolToString value)

    /// Returns kind of an attribute (i.e. is it string, boolean, enum or reference to other element).
    member this.AttributeKind element name =
        let attributeNode = attributeNode element name
        elementSemantics.AttributeValue attributeNode "kind"
*)

/// Module containing semantic operations on elements.
module private Operations =
(*
    /// Returns link corresponding to an attribute respecting generalization hierarchy.
    let rec private attributeLink element attribute =
        let thisElementAttributeLinks e =
            AttributeMetamodel.ElementSemantics.OutgoingAssociations e
            |> Seq.filter (fun a -> a.Target = Some attribute)

        let attributeLinks =
            AttributeMetamodel.ElementSemantics.Parents element
            |> Seq.map thisElementAttributeLinks
            |> Seq.concat
            |> Seq.append (thisElementAttributeLinks element)

        if Seq.isEmpty attributeLinks then
            raise (InvalidSemanticOperationException "Attribute link not found for attribute")
        elif Seq.length attributeLinks <> 1 then
            raise (InvalidSemanticOperationException "Attribute connected with multiple links to a node, model error.")
        else
            Seq.head attributeLinks

    let private copySimpleAttribute 
            (elementHelper: ElementHelper)
            (elementSemantics: Repo.AttributeMetamodel.ElementSemantics)
            element
            (``class``: IDataElement)
            name =
        let attributeClassNode = elementSemantics.Attribute ``class`` name
        let attributeAssociation =
            match name with
            | "kind" -> elementHelper.InfrastructureMetamodel.AttributeKindAssociation
            | "stringValue" -> elementHelper.InfrastructureMetamodel.AttributeStringValueAssociation
            | "isInstantiable" -> elementHelper.InfrastructureMetamodel.AttributeIsInstantiableAssociation
            | _ -> failwith "Unknown simple attribute name"

        let defaultValue = elementSemantics.AttributeValue ``class`` name
        AttributeMetamodel.ElementSemantics.AddAttribute element name attributeClassNode attributeAssociation defaultValue

    let private addAttribute
            (element: IDataElement)
            (elementHelper: ElementHelper)
            (elementSemantics: Repo.AttributeMetamodel.ElementSemantics)
            (attributeClass: IDataNode) =
        let model = AttributeMetamodel.ElementSemantics.ContainingModel element
        if elementHelper.HasAttribute element attributeClass.Name then
            let valueFromClass = elementSemantics.AttributeValue attributeClass "stringValue"
            elementHelper.SetAttributeValue element attributeClass.Name valueFromClass
        else
            let attributeLink = attributeLink element.Class attributeClass
            let attributeNode = model.CreateNode(attributeClass.Name, attributeClass)
            model.CreateAssociation(attributeLink, element, attributeNode, attributeClass.Name) |> ignore

            copySimpleAttribute elementHelper elementSemantics attributeNode attributeClass "stringValue"
            copySimpleAttribute elementHelper elementSemantics attributeNode attributeClass "kind"
            copySimpleAttribute elementHelper elementSemantics attributeNode attributeClass "isInstantiable"
*)

    /// Creates a new instance of a given class in a given model, with default values for attributes.
    let instantiate 
            (elementHelper: ElementHelper)
            (elementSemantics: Repo.AttributeMetamodel.ElementSemantics)
            (model: IDataModel)
            (ontologicalType: IDataElement) =
        if elementSemantics.StringSlotValue ontologicalType "isAbstract" <> "false" then
            raise (InvalidSemanticOperationException "Trying to instantiate abstract node")

        let name =
            match ontologicalType with
            | :? IDataNode as n -> "a" + n.Name
            | :? IDataAssociation as a -> a.TargetName
            | _ -> raise (InvalidSemanticOperationException
                    "Trying to instantiate something that should not be instantiated")

        let newElement =
            if elementSemantics.StringSlotValue ontologicalType "instanceMetatype" = "Metatype.Node" then
                model.CreateNode(name, ontologicalType, elementHelper.InfrastructureMetamodel.Node) :> IDataElement
            else
                model.CreateAssociation(
                    ontologicalType, 
                    elementHelper.InfrastructureMetamodel.Association, 
                    None, 
                    None, 
                    name
                    ) :> IDataElement

        let attributes = elementSemantics.Attributes ontologicalType

        attributes |> Seq.rev |> Seq.iter (fun a -> elementSemantics.AddSlot newElement a "")

        newElement

/// Helper class that provides low-level operations with a model conforming to Infrastructure Metamodel.
type InfrastructureSemantic(repo: IDataRepository) =
    let infrastructureMetamodel = InfrastructureMetamodel(repo)
    let elementHelper = ElementHelper(infrastructureMetamodel, repo)
    let elementSemantics = Repo.AttributeMetamodel.ElementSemantics repo

    member this.Instantiate (model: IDataModel) (ontologicalType: IDataElement) =
        Operations.instantiate elementHelper elementSemantics model ontologicalType

    member this.Metamodel = infrastructureMetamodel
    member this.Element = elementHelper
