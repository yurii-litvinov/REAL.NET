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
    let booleanNode = findNode "Boolean"

    let attributesAssociation = CoreSemanticLayer.Model.findAssociationWithSource element "attributes"
    let attributeKindAssociation = CoreSemanticLayer.Model.findAssociationWithSource attribute "kind"
    let attributeStringValueAssociation = CoreSemanticLayer.Model.findAssociationWithSource attribute "stringValue"
    let attributeIsInstantiableAssociation =
        CoreSemanticLayer.Model.findAssociationWithSource attribute "isInstantiable"

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

    /// Returns node of an attribute if it is an attribute of this element (ignoring generalization hierarchy) or None
    /// if not.
    let thisElementAttribute element name =
        thisElementAttributes element |> Seq.tryFind (fun attr -> attr.Name = name)

    /// Returns a sequence of all attributes from all parents of this element (not including element itself).
    let parentsAttributes element =
        CoreSemanticLayer.Element.parents element
        |> Seq.map thisElementAttributes
        |> Seq.concat

    /// Returns all attributes of this element, including attributes of parents in generalization hierarchy.
    let allAttributes element =
        let parentsAttributes =
            CoreSemanticLayer.Element.parents element
            |> Seq.map thisElementAttributes
            |> Seq.concat

        Seq.append (thisElementAttributes element) parentsAttributes

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
        let model = CoreSemanticLayer.Element.containingModel element
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

        CoreSemanticLayer.Element.addAttribute attribute "kind" attributeKindNode kindAssociation kind
        CoreSemanticLayer.Element.addAttribute attribute "stringValue" stringValueAssociation stringNode value
        CoreSemanticLayer.Element.addAttribute
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

            let parentAttributeValue = CoreSemanticLayer.Element.attributeValue parentAttribute "stringValue"
            let parentAttributeKind = CoreSemanticLayer.Element.attributeValue parentAttribute "kind"
            let parentAttributeIsInstantiable =
                CoreSemanticLayer.Element.attributeValue parentAttribute "isInstantiable"

            addAttribute element name parentAttributeKind parentAttributeValue parentAttributeIsInstantiable

    /// Returns underlying Infrastructure Metamodel.
    member this.InfrastructureMetamodel = infrastructureMetamodel

    /// Returns a list of all attributes for an element, respecting generalization. Attributes of most special node
    /// are the first in resulting sequence.
    /// TODO: Actually do BFS and ignore overriden attributes.
    member this.Attributes element = allAttributes element

    /// Returns true if an attribute with given name is present in given element.
    /// TODO: Actually do BFS and stop on first matching attribute.
    member this.HasAttribute element name =
        this.Attributes element |> Seq.filter (fun attr -> attr.Name = name) |> Seq.isEmpty |> not

    /// Adds a new attribute to a given element and initializes it with given value.
    member this.AddAttribute element name kind value =
        addAttribute element name kind value "true" |> ignore

    /// Returns value of an attribute with given name.
    // TODO: Actually do BFS and stop on first matching attribute.
    member this.AttributeValue element attributeName =
        // TODO: Check that it is actually last overriden attribute, not some attribute from one of the parents.
        // TODO: Also do something with correctness of attribute inheritance.
        attributeNode element attributeName
        |> fun attr -> CoreSemanticLayer.Element.attributeValue attr "stringValue"

    /// Sets value for a given attribute to a given value. Copies it from parent if needed.
    member this.SetAttributeValue element attributeName value =
        let attribute = copyIfNeeded element attributeName
        CoreSemanticLayer.Element.setAttributeValue attribute "stringValue" value

    /// Returns true if an attribute is instantiable (passes to the instances), false if instances shall not have it.
    member this.IsAttributeInstantiable element name =
        let attributeNode = attributeNode element name
        CoreSemanticLayer.Element.attributeValue attributeNode "isInstantiable" = "true"

    /// Sets this attribute to instantiable or non-instantiable.
    member this.SetAttributeInstantiable element attributeName (value: bool) =
        let boolToString v =
            match v with
            | true -> "true"
            | false -> "false"

        let attribute = copyIfNeeded element attributeName
        CoreSemanticLayer.Element.setAttributeValue attribute "isInstantiable" (boolToString value)

    /// Returns kind of an attribute (i.e. is it string, boolean, enum or reference to other element).
    member this.AttributeKind element name =
        let attributeNode = attributeNode element name
        CoreSemanticLayer.Element.attributeValue attributeNode "kind"

/// Module containing semantic operations on elements.
module private Operations =
    /// Returns link corresponding to an attribute respecting generalization hierarchy.
    let rec private attributeLink element attribute =
        let thisElementAttributeLinks e =
            CoreSemanticLayer.Element.outgoingAssociations e
            |> Seq.filter (fun a -> a.Target = Some attribute)

        let attributeLinks =
            CoreSemanticLayer.Element.parents element
            |> Seq.map thisElementAttributeLinks
            |> Seq.concat
            |> Seq.append (thisElementAttributeLinks element)

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
            | "isInstantiable" -> elementHelper.InfrastructureMetamodel.AttributeIsInstantiableAssociation
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
            copySimpleAttribute elementHelper attributeNode attributeClass "isInstantiable"

    /// Creates a new instance of a given class in a given model, with default values for attributes.
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

        let attributes =
            elementHelper.Attributes ``class``
            |> Seq.filter (fun n -> CoreSemanticLayer.Element.attributeValue n "isInstantiable" = "true")

        attributes |> Seq.rev |> Seq.iter (addAttribute newElement elementHelper)

        newElement

/// Helper class that provides low-level operations with a model conforming to Infrastructure Metmodel.
type InfrastructureSemantic(repo: IRepo) =
    let infrastructureMetamodel = InfrastructureMetamodel(repo)
    let elementHelper = ElementHelper(infrastructureMetamodel)

    member this.Instantiate (model: IModel) (``class``: IElement) =
        Operations.instantiate elementHelper model ``class``

    member this.Metamodel = infrastructureMetamodel
    member this.Element = elementHelper
