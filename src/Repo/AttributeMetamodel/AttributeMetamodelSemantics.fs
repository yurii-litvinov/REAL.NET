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
type ElementSemantics (repo: IDataRepository) =
    inherit CoreMetamodel.ElementSemantics ()

    let attributeMetamodel = repo.Model "AttributeMetamodel"

    let attribute = attributeMetamodel.Node "Attribute"
    let attributeAssociation = attributeMetamodel.Association "attributes"
    let attributeTypeAssociation = attributeMetamodel.Association "type"
    let attributeDefaultValueAssociation = attributeMetamodel.Association "defaultValue"

    let slot = attributeMetamodel.Node "Slot"
    let slotAssociation = attributeMetamodel.Association "slots"
    let slotValueAssociation = attributeMetamodel.Association "value"

    /// Returns true if given association is an attribute association (i.e. is a linguistic instance of "attributes"
    /// association from Attribute Metamodel).
    let isAttributeAssociation (association: IDataAssociation) =
        match association.LinguisticType with
        | :? IDataAssociation as a -> a = attributeAssociation
        | _ -> false

    /// Returns true if given association is a slot association (i.e. is a linguistic instance of "slots"
    /// association from Attribute Metamodel).
    let isSlotAssociation (association: IDataAssociation) =
        match association.LinguisticType with
        | :? IDataAssociation as a -> a = slotAssociation
        | _ -> false

    /// Returns slot node with given name for a given element.
    /// Throws if there is no such slot or there is more than one.
    let slotNode name element =
        let slots = 
            ElementSemantics.OutgoingAssociations element
            |> Seq.filter isSlotAssociation
            |> Seq.filter (fun a -> a.TargetName = name)

        if Seq.isEmpty slots then
            failwithf "No such slot: %s" name
        
        slots
        |> Seq.exactlyOne
        |> fun a -> a.Target
        |> Option.get

    /// Returns if a given element has attribute with given name, ignoring generalization hierarchy.
    let hasOwnAttribute name element =
        ElementSemantics.OutgoingAssociations element
        |> Seq.filter isAttributeAssociation
        |> Seq.exists (fun (e: IDataAssociation) -> e.TargetName = name)

    /// Returns all own attribute associations for a given element.
    let ownAttributeAssociations element =
        ElementSemantics.OutgoingAssociations element
        |> Seq.filter isAttributeAssociation

    /// Returns a sequence of all attribute associations for a given element.
    let attributeAssociations element =
        ElementSemantics.Parents element
        |> Seq.map ownAttributeAssociations
        |> Seq.concat
        |> Seq.append (ownAttributeAssociations element)

    /// Returns true if an attribute with given name is present in given element.
    member this.HasAttribute element name =
        bfs element isGeneralization (hasOwnAttribute name) |> Option.isSome

    /// Returns an attribute (target node of an outgoing association with given target name).
    /// Throws InvalidSemanticOperationException if there is no such association or there is more than one.
    member this.Attribute element name =
        let attributeLink: IDataAssociation = this.AttributeAssociation element name
        let result = attributeLink.Target
        if result.IsNone then
            raise (InvalidSemanticOperationException <| sprintf "Attribute link for attribute %s is unconnected" name)
        match result.Value with
        | :? IDataNode as result -> result
        | _ -> raise (InvalidSemanticOperationException <| sprintf "Attribute %s is not a node" name)

    /// Returns an outgoing association with given target name, respecting generalization hierarchy.
    /// Throws InvalidSemanticOperationException if there is no such association or there is more than one.
    member this.AttributeAssociation element name =
        let attributeLink = 
            Helpers.getExactlyOne (attributeAssociations element)
                    (fun e -> e.TargetName = name)
                    (fun () -> AttributeNotFoundException name)
                    (fun () -> MultipleAttributesException name)
        attributeLink

    /// Returns a sequence of attributes in a given element, ignoring generalization hierarchy.
    member this.OwnAttributes element = 
        ownAttributeAssociations element
        |> Seq.map (fun a -> a.Target)
        |> Seq.map (fun o -> o.Value)
        |> Seq.cast<IDataNode>

    /// Returns a list of all attribute nodes of a given element.
    member this.Attributes element =
        attributeAssociations element
        |> Seq.map (fun a -> a.Target.Value)
        |> Seq.cast<IDataNode>

    /// Adds a new attribute to an element. New attribute is always a linguistic extension (i.e. can not have 
    /// ontological type) at this metalevel.
    member this.AddAttribute element name attributeType defaultValue =
        let model = ElementSemantics.ContainingModel element
        let attributeNode = model.CreateNode(name, attribute, attribute)
        model.CreateAssociation
                (
                attributeAssociation,
                attributeAssociation,
                element,
                attributeNode,
                name
                ) |> ignore

        model.CreateAssociation
                (
                attributeTypeAssociation,
                attributeTypeAssociation,
                attributeNode,
                attributeType,
                "type"
                ) |> ignore

        model.CreateAssociation
                (
                attributeDefaultValueAssociation,
                attributeDefaultValueAssociation,
                attributeNode,
                defaultValue,
                "defaultValue"
                ) |> ignore

    /// Adds a new slot that is an instance of a given attribute. Sets its value.
    member this.AddSlot (element: IDataElement) (attribute: IDataNode) (value: IDataNode) =
        let model = ElementSemantics.ContainingModel element
        let slotNode = model.CreateNode(attribute.Name, attribute, slot)
        let attributeAssociation = attribute.IncomingEdges |> Seq.exactlyOne
        let attributeTypeAssociation = ElementSemantics.OutgoingAssociation attribute "type"

        model.CreateAssociation
                (
                attributeAssociation,
                slotAssociation,
                element,
                slotNode,
                attribute.Name
                ) |> ignore

        model.CreateAssociation
                (
                attributeTypeAssociation,
                slotValueAssociation,
                slotNode,
                value,
                "value"
                ) |> ignore

    /// Returns true if given element has given slot.
    member this.HasSlot (name: string) (element: IDataElement)  =
        ElementSemantics.OutgoingAssociations element
        |> Seq.filter isSlotAssociation
        |> Seq.filter (fun a -> a.TargetName = name)
        |> Seq.length = 1

    /// Returns slot value (an element) by given slot name.
    member this.SlotValue (name: string) (element: IDataElement)  =
        slotNode name element
        |> fun a -> a.OutgoingEdges
        |> Seq.exactlyOne
        |> fun a -> a.Target
        |> Option.get

    /// Convenience method that returns slot value as a string by given slot name.
    member this.StringSlotValue (name: string) (element: IDataElement)  =
        match this.SlotValue name element with
        | :? IDataNode as n -> n.Name
        | _ -> failwith "Slot value can only be a node"

    /// Sets new slot value, leaving old value in a model. Assumes that slot exists.
    member this.SetSlotValue (element: IDataElement) (slotName: string) (value: IDataNode) =
        let model = ElementSemantics.ContainingModel element
        let slotNode = slotNode slotName element
        let oldSlotValueAssociation = ElementSemantics.OutgoingAssociation slotNode "value"
        let attributeTypeAssociation = oldSlotValueAssociation.OntologicalType

        model.DeleteElement oldSlotValueAssociation

        model.CreateAssociation
                (
                attributeTypeAssociation,
                slotValueAssociation,
                slotNode,
                value,
                "value"
                ) |> ignore

    /// Sets new slot value as an instance of String, leaving old value in a model. Assumes that slot exists.
    member this.SetStringSlotValue (slotName: string) (element: IDataElement) (value: string) =
        let model = ElementSemantics.ContainingModel element
        let stringNode = model.LinguisticMetamodel.Node "String"
        let stringInstance = model.CreateNode(value, stringNode, stringNode)
        this.SetSlotValue element slotName stringInstance

    /// Returns a sequence of all slot nodes present in given element.
    member this.Slots (element: IDataElement) =
        ElementSemantics.OutgoingAssociations element
        |> Seq.filter isSlotAssociation
        |> Seq.map (fun a -> a.Target.Value)
        |> Seq.cast<IDataNode>

/// Helper methods for working with attributes.
type AttributeSemantics () =
    /// Returns attribute name.
    static member Name (attribute: IDataNode) =
        attribute.Name

    /// Returns a node that represents type of an attribute.
    static member Type (attribute: IDataNode) =
        ElementSemantics.OutgoingAssociation attribute "type"
        |> fun a -> a.Target.Value :?> IDataNode

    /// Returns node representing default value of an attribute.
    static member DefaultValue (attribute: IDataNode) =
        ElementSemantics.OutgoingAssociation attribute "defaultValue" |> fun a -> a.Target.Value :?> IDataNode
        
/// Helper functions for node semantics.
type NodeSemantics (repo: IDataRepository) =
    inherit CoreMetamodel.NodeSemantics ()

    let elementHelper = ElementSemantics repo

    /// Returns a node that represents slot value.
    let slotValue slot =
        ElementSemantics.OutgoingAssociation slot "value"
        |> fun a -> a.Target.Value :?> IDataNode

    /// Returns string representation of a node.
    member this.ToString (node: IDataNode) =
        let result = sprintf "Name: %s\n" <| node.Name
        let result = result + (sprintf "Ontological type: %s\n" <| node.OntologicalType.ToString ())
        let result = result + (sprintf "Linguistic type: %s\n" <| node.LinguisticType.ToString())
        let result = result + "Attributes:\n"
        let attributes =
            elementHelper.OwnAttributes node
            |> Seq.map (fun attr -> sprintf "    %s: %s\n" attr.Name (AttributeSemantics.Type attr).Name)
            |> fun s -> if Seq.isEmpty s then "" else Seq.reduce (+) s
        let result = result + attributes

        let result = result + "Slots:\n"
        let slots =
            elementHelper.Slots node
            |> Seq.map (fun slot -> sprintf "    %s = %s\n" slot.Name (slotValue slot).Name)
            |> fun s -> if Seq.isEmpty s then "" else Seq.reduce (+) s
        result + slots

/// Helper functions for working with models.
type ModelSemantics (repo: IDataRepository) =
    inherit CoreMetamodel.ModelSemantics ()

    let nodeSemantics = NodeSemantics(repo)

    /// Prints model contents on a console.
    member this.PrintContents (model: IDataModel) =
        printfn "%s (ontological metamodel: %s, linguistic metamodel: %s):" 
            model.Name
            model.OntologicalMetamodel.Name
            model.LinguisticMetamodel.Name
       
        printfn "Nodes:"
        model.Nodes
            |> Seq.map (fun n -> nodeSemantics.ToString n)
            |> Seq.iter (printfn "%s\n")
        printfn ""

        printfn "Edges:"
        model.Edges 
            |> Seq.map (fun e -> e.ToString())
            |> Seq.iter (printfn "    %s")

/// Helper class that provides semantic operations on models conforming to Attribute Metamodel.
type AttributeMetamodelSemantics(repo: IDataRepository) =
    let elementHelper = ElementSemantics(repo)

    let attributeMetamodel = repo.Model "AttributeMetamodel"
    let node = attributeMetamodel.Node "Node"
    let association = attributeMetamodel.Node "Association"

    /// Adds a new slot which is an instance of attribute with a given name to a given element and assigns 
    /// it given value. ontologicalType is an ontological type of an element.
    let instantiateAttribute element ontologicalType name value =
        if elementHelper.HasAttribute ontologicalType name then
            let attribute = elementHelper.Attribute ontologicalType name
            elementHelper.AddSlot element attribute value
        else
            raise <| AttributeNotFoundException name

    /// Adds a new instances of attributes whose names and initial values are provided in attributeValues into element.
    /// ontologicalType is an ontological type of an element.
    let instantiateAttributes (element: IDataElement) (ontologicalType: IDataElement) attributeValues =
        attributeValues |> Map.iter (instantiateAttribute element ontologicalType)

    /// Instantiates given node into given model, using given map to provide values for element attributes.
    member this.InstantiateNode
            (model: IDataModel)
            (name: string)
            (ontologicalType: IDataNode)
            (attributeValues: Map<string, IDataNode>) =
        let instance = model.CreateNode(name, ontologicalType, node)
        instantiateAttributes instance ontologicalType attributeValues
        instance

    /// Instantiates given edge into given model, using given map to provide values for element attributes.
    /// Rules for instantiation are the same as for instantiation of nodes.
    member this.InstantiateAssociation 
            (model: IDataModel)
            (source: IDataNode)
            (target: IDataNode)
            (ontologicalType: IDataElement)
            (attributeValues: Map<string, IDataNode>) =

        let name =
            match ontologicalType with
            | :? IDataNode as n -> n.Name
            | :? IDataAssociation as a -> a.TargetName
            | _ -> failwith "Incorrect association ontological type"

        let instance = 
            model.CreateAssociation (
                ontologicalType, 
                association, 
                source, 
                target, 
                name
            )

        instantiateAttributes instance ontologicalType attributeValues
        instance
