(* Copyright 2017-2019 Yurii Litvinov
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

namespace Repo.CoreModel

open Repo
open Repo.DataLayer

/// Helper functions for element semantics.
module Element =
    open System.Collections.Generic

    /// Does a breadth-first search from a given element following given interesting edges until element matching
    /// given predicate is found.
    let private bfs (element: IDataElement) 
            (isInterestingEdge: IDataEdge -> bool) 
            (isWhatWeSearch: IDataElement -> bool) =
        let queue = Queue<IDataElement>()
        queue.Enqueue element
        let visited = HashSet<IDataElement>()
        let rec doBfs () =
            if queue.Count = 0 then
                None
            else
                let currentElement = queue.Dequeue ()
                if isWhatWeSearch currentElement then
                    Some currentElement
                else
                    visited.Add currentElement |> ignore
                    currentElement.OutgoingEdges
                    |> Seq.filter isInterestingEdge
                    |> Seq.map (fun e -> e.Target)
                    |> Seq.choose id
                    |> Seq.filter (not << visited.Contains)
                    |> Seq.iter queue.Enqueue
                    doBfs()

        doBfs ()

    /// Returns true if given element is generalization edge.
    let private isGeneralization (e: IDataElement) = e :? IDataGeneralization

    /// Returns true if given element is association edge.
    let private isAssociation (e: IDataElement) = e :? IDataAssociation

    /// Returns a model containing given element.
    let containingModel (element: IDataElement) =
        element.Model

    /// Returns all outgoing edges for an element.
    let outgoingEdges (element: IDataElement) =
        element.OutgoingEdges

    /// Returns all outgoing generalizations for an element.
    let outgoingGeneralizations element =
        outgoingEdges element |> Seq.filter isGeneralization |> Seq.cast<IDataGeneralization>

    /// Returns all outgoing associations for an element.
    let outgoingAssociations element =
        outgoingEdges element |> Seq.filter isAssociation |> Seq.cast<IDataAssociation>

    /// Returns a sequence of all parents (in terms of generalization hierarchy) for given element.
    /// Most special node is the first in resulting sequence, most general is the last.
    let rec parents element =
        outgoingGeneralizations element
        |> Seq.map (fun g -> g.Target)
        |> Seq.choose id
        |> Seq.map (fun p -> Seq.append (Seq.singleton p) (parents p))
        |> Seq.concat

    /// Returns a sequence of attributes in a given element, ignoring generalization hierarchy.
    let strictElementAttributes element =
        outgoingAssociations element

    /// Returns if a given element has attribute with given name, ignoring generalization hierarchy.
    let private hasStrictAttribute name element =
        outgoingAssociations element
        |> Seq.exists (fun e -> e.TargetName = name)

    /// Searches for attribute in generalization hierarchy.
    let attributes element =
        parents element
        |> Seq.map strictElementAttributes
        |> Seq.concat
        |> Seq.append (strictElementAttributes element)

    /// Returns an attribute lint (outgoing association with given target name).
    /// Throws InvalidSemanticOperationException if there is no such association or there is more than one.
    let attributeLink element name =
        let attributeContainer = bfs element isGeneralization (hasStrictAttribute name)
        if attributeContainer.IsNone then
            raise (AttributeNotFoundException name)
        let attributeLink = 
            Helpers.getExactlyOne (outgoingAssociations attributeContainer.Value)
                    (fun e -> e.TargetName = name)
                    (fun () -> AttributeNotFoundException name)
                    (fun () -> MultipleAttributesException name)
        attributeLink

    /// Returns an attribute (target node of an outgoing association with given target name).
    /// Throws InvalidSemanticOperationException if there is no such association or there is more than one.
    let attribute element name =
        let attributeLink = attributeLink element name
        let result = attributeLink.Target
        if result.IsNone then
            raise (InvalidSemanticOperationException <| sprintf "Attribute link for attribute %s is unconnected" name)
        match result.Value with
        | :? IDataNode as result -> result
        | _ -> raise (InvalidSemanticOperationException
            <| sprintf "Attribute %s is not a node (which is possible but not used and not supported in v1)" name)

    /// Returns string value of a given attribute.
    let attributeValue element name =
        (attribute element name).Name

    /// Adds a new attribute with a given value to an element.
    let addAttribute element name (``class``: IDataElement) attributeAssociationClass value =
        let model = containingModel element
        let attributeNode = model.CreateNode(value, ``class``)
        model.CreateAssociation(attributeAssociationClass, element, attributeNode, name) |> ignore

    /// Sets attribute value for given attribute. If this attribute is defined in parent, copies it into current
    /// element and then sets value.
    let setAttributeValue element name value =
        let strictAtribute = strictElementAttributes element |> Seq.tryFind (fun attr -> attr.TargetName = name)
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
                strictElementAttributes parentWithAttribute |> Seq.find (fun a -> a.TargetName = name)
            if parentAttributeEdge.Target.IsNone || not (parentAttributeEdge.Target.Value :? IDataNode) then
                raise (InvalidSemanticOperationException
                    <| sprintf "Attribute %s is not connected or not a node" name)
            let parentAttributeNode = parentAttributeEdge.Target.Value :?> IDataNode
            addAttribute element name parentAttributeNode.Class parentAttributeEdge.Class value

    /// Returns true if an attribute with given name is present in given element.
    let hasAttribute element name =
        bfs element isGeneralization (hasStrictAttribute name) |> Option.isSome

    /// Returns true if 'descendant' is a (possibly indirect) descendant of a 'parent', in terms of generalization
    /// hierarchy.
    let rec isDescendantOf (parent: IDataElement) (descendant: IDataElement) =
        bfs descendant isGeneralization ((=) parent) |> Option.isSome

    /// Returns true if an 'instance' is a (possibly indirect) instance of a 'class'.
    let rec isInstanceOf (``class``: IDataElement) (instance: IDataElement) =
        if instance.Class = ``class`` || isDescendantOf ``class`` instance.Class then
            true
        elif instance.Class = instance then
            false
        else
            isInstanceOf ``class`` instance.Class

/// Helper functions for node semantics.
module Node =
    /// Returns name of a node.
    /// Throws InvalidSemanticOperationException if given element is not node so it does not have a name.
    let name (element: IDataElement) =
        if not <| element :? IDataNode then
            raise (InvalidSemanticOperationException "Only nodes have a name in REAL.NET")
        (element :?> IDataNode).Name

    /// Sets name of a node.
    /// Throws InvalidSemanticOperationException if given element is not node so it does not have a name.
    let setName name (element: IDataElement) =
        if not <| element :? IDataNode then
            raise (InvalidSemanticOperationException "Only nodes have a name in REAL.NET")
        (element :?> IDataNode).Name <- name
    
    /// Returns string representation of a node.
    let toString (node: IDataNode) =
        let result = sprintf "Name: %s\n" <| node.Name
        let result = result + (sprintf "Class: %s\n" <| name node.Class)
        let result = result + "Attributes:\n"
        let attributes =
            Element.strictElementAttributes node
            |> Seq.map (fun attr -> sprintf "    %s = %s\n" attr.TargetName (name attr.Target.Value))
            |> Seq.reduce (+)
        result + attributes


/// Helper functions for working with models.
module Model =
    /// Searches for a given node in a given model by name, returns None if not found or found more than one.
    let tryFindNode (model: IDataModel) name =
        if model.HasNode name then
            Some <| model.Node name
        else
            None

    /// Searches for a given association in a given model by target name and additional predicate. Assumes that it
    /// exists and there is only one such association. Throws InvalidSemanticOperationException if not.
    let private findAssociationIn (edges: IDataEdge seq) targetName =
        Helpers.getExactlyOne edges
                (fun e -> e :? IDataAssociation && (e :?> IDataAssociation).TargetName = targetName)
                (fun () -> InvalidSemanticOperationException 
                        <| sprintf "Edge %s not found" targetName)
                (fun () -> InvalidSemanticOperationException 
                        <| sprintf "Edge %s appears more than once" targetName)
        :?> IDataAssociation

    /// Searches for a given association in a given model by target name. Assumes that it exists and there is only one
    /// association with that name. Throws InvalidSemanticOperationException if not.
    let findAssociation (model: IDataModel) targetName =
        findAssociationIn model.Edges targetName

    /// Searches for a given association starting in a given element with a given name. Assumes that it exists and
    /// there is only one association with that name. Throws InvalidSemanticOperationException if not.
    let findAssociationWithSource (element: IDataElement) targetName =
        findAssociationIn element.OutgoingEdges targetName

/// Helper class that provides semantic operations on models conforming to Core Metamodel.
type CoreSemantics(repo: IDataRepository) =
    /// Adds a new instance of an attribute with a given name to a given element and assigns it given value.
    /// ``class`` is a class of an element.
    let instantiateAttribute element ``class`` name value =
        if Element.hasAttribute ``class`` name then 
            let associationType = Element.attributeLink ``class`` name
            let attributeType = associationType.Target.Value
            if not <| (Seq.isEmpty <| Element.outgoingEdges attributeType) then
                raise <| InvalidSemanticOperationException("Trying to set simple value to an attribute that " + 
                        "looks like complex object")
            Element.addAttribute 
                element 
                associationType.TargetName 
                attributeType 
                associationType 
                value
        else
            raise <| AttributeNotFoundException name

    /// Adds a new instances of attributes whose names and initial values are provided in attributeValues into element.
    /// ``class`` is a class of an element.
    let instantiateAttributes element ``class`` attributeValues =
        attributeValues |> Map.iter (instantiateAttribute element ``class``)

    /// Instantiates given node into given model, using given map to provide values for element attributes.
    /// Since Core metamodel is rather limited, there are some counter-intuitive considerations:
    /// - actually there is no notion of attribute in Core metamodel, attributes are no more than associations
    ///   pointing to nodes which name is considered attribute value;
    /// - model has no way to know if given association is an attribute or a proper association with a separate node;
    /// - Core metamodel assumes that each association has unbounded multiplicity, so it can be instantiated zero or
    ///   more times in instances.
    /// Considering this, Core semantic instantiation rules are as follows:
    /// - the result of an operation is a new node in a given model whose class is a node passed as ``class`` 
    ///   parameter;
    /// - no associations from class are instantiated (since they are unbounded they are assumed to be instantiated 0
    ///   times);
    /// - attribute names from attributeValues parameter are checked against associations of a class, if 
    ///   corresponding association found, it becomes instantiated pointing to a new node with name set to attribute
    ///   value;
    /// - if there is no association with given name in a class, exception will be thrown;
    /// - if some associations in a class did not receive their values, they are ignored and not copied into instance
    ///   (model author can manually instantiate them later).
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
