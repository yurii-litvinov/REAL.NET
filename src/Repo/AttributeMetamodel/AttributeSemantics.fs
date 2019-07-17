(* Copyright 2019 Yurii Litvinov
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

namespace Repo.AttributeMetamodel

open Repo
open Repo.DataLayer
open Repo.CoreMetamodel.CoreSemanticsHelpers

/// Helper functions for element semantics.
type Element (repo: IDataRepository) =
    inherit CoreMetamodel.Element ()

    let attributeMetamodel = repo.Model "AttributeMetamodel"
    let attribute = attributeMetamodel.Node "Attribute"
    let attributeAssociation = attributeMetamodel.Association "attributes"

    /// Returns true, if given association is an attribute association (i.e. is a linguistic instance of "attributes"
    /// association from Attribute Metamodel).
    let isAttributeAssociation (association: IDataAssociation) =
        match association.Class with
        | :? IDataAssociation as a -> a = attributeAssociation
        | _ -> false

    /// Returns if a given element has attribute with given name, ignoring generalization hierarchy.
    let hasStrictAttribute name element =
        Element.OutgoingAssociations element
        |> Seq.filter isAttributeAssociation
        |> Seq.exists (fun (e: IDataAssociation) -> e.TargetName = name)

    /// Returns a sequence of all attributes for a given element.
    let attributes element =
        Element.Parents element
        |> Seq.map Element.StrictElementAttributes
        |> Seq.concat
        |> Seq.append (Element.StrictElementAttributes element)

    /// Returns true if an attribute with given name is present in given element.
    member this.HasAttribute element name =
        bfs element isGeneralization (hasStrictAttribute name) |> Option.isSome

    /// Returns an attribute (target node of an outgoing association with given target name).
    /// Throws InvalidSemanticOperationException if there is no such association or there is more than one.
    member this.Attribute element name =
        let attributeLink: IDataAssociation = this.AttributeAssociation element name
        let result = attributeLink.Target
        if result.IsNone then
            raise (InvalidSemanticOperationException <| sprintf "Attribute link for attribute %s is unconnected" name)
        match result.Value with
        | :? IDataNode as result -> result
        | _ -> raise (InvalidSemanticOperationException
            <| sprintf "Attribute %s is not a node (which is possible but not used and not supported in v1)" name)

    /// Returns string value of a given attribute.
    member this.AttributeValue element name =
        (this.Attribute element name).Name

    /// Sets attribute value for given attribute. If this attribute is defined in parent, copies it into current
    /// element and then sets value.
    /// If this attribute is used by more than one element, exception will be thrown (it shall be treated not as an
    /// attribute, but as an association).
    member this.SetAttributeValue element name value =
        let strictAtribute = Element.StrictElementAttributes element |> Seq.tryFind (fun (attr: IDataAssociation) -> attr.TargetName = name)
        if strictAtribute.IsSome then
            match strictAtribute.Value.Target with
            | Some(e) when (e :? IDataNode) -> 
                if (e.IncomingEdges |> Seq.length) > 1 then
                    raise (InvalidSemanticOperationException <| "Attribute " + name + " is shared, " +
                            "changing value not possible")
                (e :?> IDataNode).Name <- value
            | _ -> raise (InvalidSemanticOperationException
                <| sprintf "Attribute %s is not connected or not a node" name)
        else
            let parentWithAttribute = bfs element isGeneralization (hasStrictAttribute name)
            if parentWithAttribute.IsNone then
                raise (AttributeNotFoundException name)
            let parentWithAttribute = parentWithAttribute.Value
            let parentAttributeEdge =
                Element.StrictElementAttributes parentWithAttribute |> Seq.find (fun a -> a.TargetName = name)
            if parentAttributeEdge.Target.IsNone || not (parentAttributeEdge.Target.Value :? IDataNode) then
                raise (InvalidSemanticOperationException
                    <| sprintf "Attribute %s is not connected or not a node" name)
            let parentAttributeNode = parentAttributeEdge.Target.Value :?> IDataNode
            Element.AddAttribute element name parentAttributeNode.Class parentAttributeEdge.Class value

    /// Returns an outgoing association with given target name, respecting generalization hierarchy.
    /// Throws InvalidSemanticOperationException if there is no such association or there is more than one.
    member this.AttributeAssociation element name =
        let attributeContainer = bfs element isGeneralization (hasStrictAttribute name)
        if attributeContainer.IsNone then
            raise (AttributeNotFoundException name)
        let attributeLink = 
            Helpers.getExactlyOne (Element.OutgoingAssociations attributeContainer.Value)
                    (fun e -> e.TargetName = name)
                    (fun () -> AttributeNotFoundException name)
                    (fun () -> MultipleAttributesException name)
        attributeLink

    /// Adds a new attribute with a given value to an element.
    static member AddAttribute element name (``class``: IDataElement) attributeAssociationClass value =
        let model = Element.ContainingModel element
        let attributeNode = model.CreateNode(value, ``class``)
        model.CreateAssociation(attributeAssociationClass, element, attributeNode, name) |> ignore

    /// Returns a sequence of attributes in a given element, ignoring generalization hierarchy.
    static member StrictElementAttributes element =
        Element.OutgoingAssociations element

/// Helper functions for node semantics.
type Node () =
    inherit CoreMetamodel.Node ()

    /// Returns string representation of a node.
    static member ToString (node: IDataNode) =
        let result = sprintf "Name: %s\n" <| node.Name
        let result = result + (sprintf "Class: %s\n" <| Node.Name node.Class)
        let result = result + "Attributes:\n"
        let attributes =
            Element.StrictElementAttributes node
            |> Seq.map (fun attr -> sprintf "    %s = %s\n" attr.TargetName (Node.Name attr.Target.Value))
            |> Seq.reduce (+)
        result + attributes


/// Helper functions for working with models.
type Model () =
    inherit CoreMetamodel.Model ()

/// Helper class that provides semantic operations on models conforming to Attribute Metamodel.
type AttributeSemantics(repo: IDataRepository) =
    let elementHelper = Element(repo)

    /// Adds a new instance of an attribute with a given name to a given element and assigns it given value.
    /// ``class`` is a class of an element.
    let instantiateAttribute element ``class`` name value =
        if elementHelper.HasAttribute ``class`` name then 
            let associationType = elementHelper.AttributeAssociation ``class`` name
            let attributeType = associationType.Target.Value
            if not <| (Seq.isEmpty <| Element.OutgoingEdges attributeType) then
                raise <| InvalidSemanticOperationException("Trying to set simple value to an attribute that " + 
                        "looks like complex object")
            Element.AddAttribute 
                element 
                associationType.TargetName 
                attributeType 
                associationType 
                value
        else
            raise <| AttributeNotFoundException name

    /// Adds a new instances of attributes whose names and initial values are provided in attributeValues into element.
    /// ``class`` is a class of an element.
    let instantiateAttributes (element: IDataElement) (``class``: IDataElement) attributeValues =
        attributeValues |> Map.iter (instantiateAttribute element ``class``)

    /// Instantiates given node into given model, using given map to provide values for element attributes.
    member this.InstantiateNode (model: IDataModel) (``class``: IDataNode) (attributeValues: Map<string, string>) =
        let instance = model.CreateNode("a" + ``class``.Name, ``class``)
        instantiateAttributes instance ``class`` attributeValues
        instance

    /// Instantiates given edge into given model, using given map to provide values for element attributes.
    /// Rules for instantiation are the same as for instantiation of nodes.
    member this.InstantiateAssociation 
            (model: IDataModel) 
            (source: IDataNode) 
            (target: IDataNode) 
            (``class``: IDataAssociation) 
            (attributeValues: Map<string, string>) =
        let instance = model.CreateAssociation(``class``, source, target, ``class``.TargetName)
        instantiateAttributes instance ``class`` attributeValues
        instance
