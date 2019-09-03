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

/// Helper class for working with Infrastructure Metamodel. Provides most commonly used metamodel nodes and edges.
type InfrastructureMetamodel(repo: IDataRepository) =
    let infrastructureMetametamodel = repo.Model Consts.infrastructureMetametamodel
    
    let findNode name = infrastructureMetametamodel.Node name

    let element = findNode "Element"
    let node = findNode "Node"
    let association = findNode "Association"
    let generalization = findNode "Generalization"
    let attribute = findNode "Attribute"
    let slot = findNode "Slot"
    let stringNode = findNode "String"
    let booleanNode = findNode "Boolean"
    let intNode = findNode "Int"

    let attributesAssociation = CoreMetamodel.ModelSemantics.FindAssociationWithSource element "attributes"
    let attributeTypeAssociation = CoreMetamodel.ModelSemantics.FindAssociationWithSource attribute "type"
    let attributeDefaultValueAssociation = 
        CoreMetamodel.ModelSemantics.FindAssociationWithSource attribute "defaultValue"

    let slotsAssociation = AttributeMetamodel.ModelSemantics.FindAssociationWithSource element "slots"
    let valueAssociation = AttributeMetamodel.ModelSemantics.FindAssociationWithSource slot "value"

    let isLinguisticType (element: IDataElement) (linguisticType: IDataElement) =
        element.LinguisticType = linguisticType

    member this.Model = infrastructureMetametamodel

    member this.Node = node

    member this.Association = association

    member this.Generalization = generalization

    member this.Attribute = attribute
    member this.String = stringNode
    member this.Boolean = booleanNode

    member this.AttributesAssociation = attributesAssociation
    member this.AttributeTypeAssociation = attributeTypeAssociation
    member this.AttributeDefaultValueAssociation = attributeDefaultValueAssociation

    member this.IsFromInfrastructureMetamodel element =
        CoreMetamodel.ElementSemantics.ContainingModel element = infrastructureMetametamodel

    member this.IsNode element =
        isLinguisticType element <| this.Model.LinguisticMetamodel.Node "Node"

    member this.IsAssociation element =
        isLinguisticType element <| this.Model.LinguisticMetamodel.Node "Association"

    member this.IsGeneralization element =
        isLinguisticType element <| this.Model.LinguisticMetamodel.Node "Generalization"

    member this.IsEdge element =
        this.IsAssociation element || this.IsGeneralization element

    member this.IsElement element =
        this.IsNode element || this.IsEdge element

    member this.IsAttribute element =
        isLinguisticType element <| this.Model.Node "Attribute"

    member this.IsSlot element =
        isLinguisticType element <| this.Model.Node "Slot"

    member this.IsAttributeAssociation association =
        isLinguisticType association attributesAssociation

    member this.IsAttributeTypeAssociation association =
        isLinguisticType association attributeTypeAssociation

    member this.IsAttributeDefaultValueAssociation association =
        isLinguisticType association attributeDefaultValueAssociation

    member this.IsSlotAssociation association =
        isLinguisticType association slotsAssociation

    member this.IsSlotValueAssociation association =
        isLinguisticType association valueAssociation

/// Helper for working with elements in Infrastructure Metamodel terms.
type ElementSemantics(repo: IDataRepository) =
    let infrastructureMetametamodel = new InfrastructureMetamodel(repo)
    let attribute = infrastructureMetametamodel.Attribute
    let attributeAssociation = infrastructureMetametamodel.AttributesAssociation
    let attributeTypeAssociation = infrastructureMetametamodel.AttributeTypeAssociation
    let attributeDefaultValueAssociation = infrastructureMetametamodel.AttributeDefaultValueAssociation

    let slot = infrastructureMetametamodel.Model.Node "Slot"
    let slotAssociation = infrastructureMetametamodel.Model.Association "slots"
    let slotValueAssociation = infrastructureMetametamodel.Model.Association "value"

    /// Returns true if given association is an attribute association (i.e. is a linguistic instance of "attributes"
    /// association from Infrastructure Metametamodel).
    let isAttributeAssociation (association: IDataAssociation) =
        association.LinguisticType = (attributeAssociation :> IDataElement)

    /// Returns true if given association is a slot association (i.e. is a linguistic instance of "slots"
    /// association from Infrastructure Metametamodel).
    let isSlotAssociation (association: IDataAssociation) =
        association.LinguisticType = (slotAssociation :> IDataElement)

    /// Returns slot node with given name for a given element.
    /// Throws if there is no such slot or there is more than one.
    let slotNode name element =
        let slots = 
            CoreMetamodel.ElementSemantics.OutgoingAssociations element
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
        CoreMetamodel.ElementSemantics.OutgoingAssociations element
        |> Seq.filter isAttributeAssociation
        |> Seq.exists (fun (e: IDataAssociation) -> e.TargetName = name)

    /// Returns all own attribute associations for a given element.
    let ownAttributeAssociations element =
        CoreMetamodel.ElementSemantics.OutgoingAssociations element
        |> Seq.filter isAttributeAssociation

    /// Returns a sequence of all attribute associations for a given element.
    let attributeAssociations element =
        CoreMetamodel.ElementSemantics.Parents element
        |> Seq.map ownAttributeAssociations
        |> Seq.concat
        |> Seq.append (ownAttributeAssociations element)

    /// Returns true if an attribute with given name is present in given element.
    member this.HasAttribute name element =
        Repo.CoreMetamodel.CoreSemanticsHelpers.bfs 
            element 
            Repo.CoreMetamodel.CoreSemanticsHelpers.isGeneralization (hasOwnAttribute name) 
        |> Option.isSome

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
        let model = CoreMetamodel.ElementSemantics.ContainingModel element
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
        let model = CoreMetamodel.ElementSemantics.ContainingModel element
        let slotNode = model.CreateNode(attribute.Name, attribute, slot)
        let attributeAssociation = attribute.IncomingEdges |> Seq.exactlyOne
        let attributeTypeAssociation = CoreMetamodel.ElementSemantics.OutgoingAssociation attribute "type"

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
        CoreMetamodel.ElementSemantics.OutgoingAssociations element
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
        let model = CoreMetamodel.ElementSemantics.ContainingModel element
        let slotNode = slotNode slotName element
        let oldSlotValueAssociation = CoreMetamodel.ElementSemantics.OutgoingAssociation slotNode "value"
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
        let model = CoreMetamodel.ElementSemantics.ContainingModel element
        let stringNode = model.LinguisticMetamodel.Node "String"
        let stringInstance = model.CreateNode(value, stringNode, stringNode)
        this.SetSlotValue element slotName stringInstance

    /// Returns a sequence of all slot nodes present in given element.
    member this.Slots (element: IDataElement) =
        CoreMetamodel.ElementSemantics.OutgoingAssociations element
        |> Seq.filter isSlotAssociation
        |> Seq.map (fun a -> a.Target.Value)
        |> Seq.cast<IDataNode>

    /// Returns underlying Infrastructure Metamodel.
    member this.InfrastructureMetamodel = infrastructureMetametamodel

/// Helper functions for node semantics.
type NodeSemantics (repo: IDataRepository) =
    inherit AttributeMetamodel.NodeSemantics (repo)

    let elementSemantics = ElementSemantics repo

    /// Returns a node that represents slot value.
    let slotValue slot =
        CoreMetamodel.ElementSemantics.OutgoingAssociation slot "value"
        |> fun a -> a.Target.Value :?> IDataNode

    /// Returns string representation of a node.
    member this.ToString (node: IDataNode) =
        let result = sprintf "Name: %s\n" <| node.Name
        let result = result + (sprintf "Ontological type: %s\n" <| node.OntologicalType.ToString ())
        let result = result + (sprintf "Linguistic type: %s\n" <| node.LinguisticType.ToString())
        let result = result + "Attributes:\n"
        let attributes =
            elementSemantics.OwnAttributes node
            |> Seq.map 
                (fun attr -> sprintf "    %s: %s\n" attr.Name (AttributeMetamodel.AttributeSemantics.Type attr).Name)
            |> fun s -> if Seq.isEmpty s then "" else Seq.reduce (+) s
        let result = result + attributes

        let result = result + "Slots:\n"
        let slots =
            elementSemantics.Slots node
            |> Seq.map (fun slot -> sprintf "    %s = %s\n" slot.Name (slotValue slot).Name)
            |> fun s -> if Seq.isEmpty s then "" else Seq.reduce (+) s
        result + slots

/// Helper functions for working with models.
type ModelSemantics (repo: IDataRepository) =
    inherit CoreMetamodel.ModelSemantics ()

    let infrastructureMetamodel = InfrastructureMetamodel(repo)
    let nodeSemantics = NodeSemantics(repo)

    /// Prints model contents on a console.
    member this.PrintContents (model: IDataModel) =
        printfn "%s (ontological metamodel: %s, linguistic metamodel: %s):" 
            model.Name
            model.OntologicalMetamodel.Name
            model.LinguisticMetamodel.Name
       
        printfn "Nodes:"
        model.Nodes
            |> Seq.filter (fun n -> not <| infrastructureMetamodel.IsAttribute n)
            |> Seq.filter (fun n -> not <| infrastructureMetamodel.IsSlot n)
            |> Seq.map (fun n -> nodeSemantics.ToString n)
            |> Seq.iter (printfn "%s\n")
        printfn ""

        printfn "Edges:"
        model.Edges 
            |> Seq.filter (fun n -> not <| infrastructureMetamodel.IsAttributeAssociation n)
            |> Seq.filter (fun n -> not <| infrastructureMetamodel.IsAttributeTypeAssociation n)
            |> Seq.filter (fun n -> not <| infrastructureMetamodel.IsAttributeDefaultValueAssociation n)
            |> Seq.filter (fun n -> not <| infrastructureMetamodel.IsSlotAssociation n)
            |> Seq.filter (fun n -> not <| infrastructureMetamodel.IsSlotValueAssociation n)
            |> Seq.map (fun e -> e.ToString())
            |> Seq.iter (printfn "    %s")

/// Helper class that provides low-level operations with a model conforming to Infrastructure Metamodel.
type InfrastructureMetamodelSemantics(repo: IDataRepository) =
    let infrastructureMetamodel = InfrastructureMetamodel(repo)
    let elementSemantics = ElementSemantics(repo)

    /// Instantiates given node into given model, using given map to provide values for element attributes.
    member this.InstantiateNode
            (model: IDataModel)
            (name: string)
            (ontologicalType: IDataNode)
            (attributeValues: Map<string, IDataNode>) =
        if elementSemantics.StringSlotValue Consts.isAbstract ontologicalType <> Consts.stringFalse then
            raise (InvalidSemanticOperationException "Trying to instantiate abstract node")

        let newElement =
            if elementSemantics.StringSlotValue Consts.instanceMetatype ontologicalType = Consts.metatypeNode then
                model.CreateNode(name, ontologicalType, elementSemantics.InfrastructureMetamodel.Node) :> IDataElement
            else
                model.CreateAssociation(
                    ontologicalType, 
                    elementSemantics.InfrastructureMetamodel.Association, 
                    None, 
                    None, 
                    name
                    ) :> IDataElement

        let attributes = elementSemantics.Attributes ontologicalType

        let slotValue (attr: IDataNode) =
            let slotName = AttributeMetamodel.AttributeSemantics.Name attr
            if attributeValues.ContainsKey slotName then 
                attributeValues.[slotName]
            else
                AttributeMetamodel.AttributeSemantics.DefaultValue attr

        attributes 
        |> Seq.rev 
        |> Seq.iter (fun a -> elementSemantics.AddSlot newElement a (slotValue a))

        newElement

    /// Instantiates a node to a given model, using default name and default values for all attributes.
    member this.Instantiate (model: IDataModel) (ontologicalType: IDataElement) =
        let name =
            match ontologicalType with
            | :? IDataNode as n -> "a" + n.Name
            | :? IDataAssociation as a -> a.TargetName
            | _ -> raise (InvalidSemanticOperationException
                    "Trying to instantiate something that should not be instantiated")

        this.InstantiateNode model name (ontologicalType :?> IDataNode) Map.empty

    member this.Metamodel = infrastructureMetamodel
    member this.Element = elementSemantics
