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

namespace Repo.CoreSemanticLayer

open Repo
open Repo.DataLayer

/// Helper functions for working with repository.
module Repo =
    /// Searches model by name in a repository, throws InvalidSemanticOperationException if not found or multiple
    /// models found.
    let findModel (repo: IRepo) (name: string) =
        let models =
            repo.Models
            |> Seq.filter (fun m -> m.Name = name)

        if Seq.isEmpty models then
            raise (ModelNotFoundException name)
        elif Seq.length models <> 1 then
            raise (MultipleModelsException name)
        else
            Seq.head models

    /// Searches for a Core Metamodel in current repository.
    let coreMetamodel (repo: IRepo) =
        findModel repo "CoreMetametamodel"

/// Helper functions for element semantics.
module Element =
    open System.Collections.Generic

    /// Does a breadth-first search from a given element following given interesting edges until element matching
    /// given predicate is found.
    let private bfs (element: IElement) (isInterestingEdge: IEdge -> bool) (isWhatWeSearch: IElement -> bool) =
        let queue = Queue<IElement>()
        queue.Enqueue element
        let visited = HashSet<IElement>()
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
    let private isGeneralization (e: IElement) = e :? IGeneralization

    /// Returns true if given element is association edge.
    let private isAssociation (e: IElement) = e :? IAssociation

    /// Returns a model containing given element.
    let containingModel (element: IElement) =
        element.Model

    /// Returns all outgoing edges for an element.
    let outgoingEdges (element: IElement) =
        element.OutgoingEdges

    /// Returns all outgoing generalizations for an element.
    let outgoingGeneralizations element =
        outgoingEdges element |> Seq.filter isGeneralization |> Seq.cast<IGeneralization>

    /// Returns all outgoing associations for an element.
    let outgoingAssociations element =
        outgoingEdges element |> Seq.filter isAssociation |> Seq.cast<IAssociation>

    /// Returns a sequence of all parents (in terms of generalization hierarchy) for given element.
    /// Most special node is the first in resulting sequence, most general is the last.
    let rec parents element =
        outgoingGeneralizations element
        |> Seq.map (fun g -> g.Target)
        |> Seq.choose id
        |> Seq.map (fun p -> Seq.append (Seq.singleton p) (parents p))
        |> Seq.concat

    /// Returns a sequence of attributes in a given element, ignoring generalization hierarchy.
    let private strictElementAttributes element =
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

    /// Returns an attribute (target node of an outgoing association with given target name).
    /// Throws InvalidSemanticOperationException if there is no such association or there is more than one.
    let rec attribute element name =
        let attributeContainer = bfs element isGeneralization (hasStrictAttribute name)
        if attributeContainer.IsNone then
            raise (AttributeNotFoundException name)
        let attributeLink = outgoingAssociations attributeContainer.Value |> Seq.find (fun e -> e.TargetName = name)
        let result = attributeLink.Target
        if result.IsNone then
            raise (InvalidSemanticOperationException <| sprintf "Attribute link for attribute %s is unconnected" name)
        match result.Value with
        | :? INode as result -> result
        | _ -> raise (InvalidSemanticOperationException
            <| sprintf "Attribute %s is not a node (which is possible but not used and not supported in v1)" name)

    /// Returns string value of a given attribute.
    let attributeValue element name =
        (attribute element name).Name

    /// Adds a new attribute with a given value to an element.
    let addAttribute element name (``class``: IElement) attributeAssociationClass value =
        let model = containingModel element
        let attributeNode = model.CreateNode(value, ``class``)
        model.CreateAssociation(attributeAssociationClass, element, attributeNode, name) |> ignore

    /// Sets attribute value for given attribute. If this attribute is defined in parent, copies it into current
    /// element and then sets value.
    let setAttributeValue element name value =
        let strictAtribute = strictElementAttributes element |> Seq.tryFind (fun attr -> attr.TargetName = name)
        if strictAtribute.IsSome then
            match strictAtribute.Value.Target with
            | Some(e) when (e :? INode) -> (e :?> INode).Name <- value
            | _ -> raise (InvalidSemanticOperationException
                <| sprintf "Attribute %s is not connected or not a node" name)
        else
            let parentWithAttribute = bfs element isGeneralization (hasStrictAttribute name)
            if parentWithAttribute.IsNone then
                raise (AttributeNotFoundException name)
            let parentWithAttribute = parentWithAttribute.Value
            let parentAttributeEdge =
                strictElementAttributes parentWithAttribute |> Seq.find (fun a -> a.TargetName = name)
            if parentAttributeEdge.Target.IsNone || not (parentAttributeEdge.Target.Value :? INode) then
                raise (InvalidSemanticOperationException
                    <| sprintf "Attribute %s is not connected or not a node" name)
            let parentAttributeNode = parentAttributeEdge.Target.Value :?> INode
            addAttribute element name parentAttributeNode.Class parentAttributeEdge.Class value

    /// Returns true if an attribute with given name is present in given element.
    let hasAttribute element name =
        bfs element isGeneralization (hasStrictAttribute name) |> Option.isSome

    /// Returns true if 'descendant' is a (possibly indirect) descendant of a 'parent', in terms of generalization
    /// hierarchy.
    let rec isDescendantOf (parent: IElement) (descendant: IElement) =
        bfs descendant isGeneralization ((=) parent) |> Option.isSome

    /// Returns true if an 'instance' is a (possibly indirect) instance of a 'class'.
    let rec isInstanceOf (``class``: IElement) (instance: IElement) =
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
    let name (element: IElement) =
        if not <| element :? INode then
            raise (InvalidSemanticOperationException "Only nodes have a name in REAL.NET")
        (element :?> INode).Name

    /// Sets name of a node.
    /// Throws InvalidSemanticOperationException if given element is not node so it does not have a name.
    let setName name (element: IElement) =
        if not <| element :? INode then
            raise (InvalidSemanticOperationException "Only nodes have a name in REAL.NET")
        (element :?> INode).Name <- name

/// Helper functions for working with models.
module Model =
    /// Searches for a given node in a given model by name. Assumes that it exists and there is only one node with
    /// that name.
    let findNode (model: IModel) name =
        let nodes = model.Nodes |> Seq.filter (fun m -> m.Name = name)

        if Seq.isEmpty nodes then
            raise (InvalidSemanticOperationException <| sprintf "Node %s not found in model %s" name model.Name)
        elif Seq.length nodes <> 1 then
            raise (InvalidSemanticOperationException
                <| sprintf "Node %s appears more than once in model %s" name model.Name)
        else
            Seq.head nodes

    /// Searches for a given node in a given model by name, returns None if not found or found more than one.
    let tryFindNode (model: IModel) name =
        let nodes = model.Nodes |> Seq.filter (fun m -> m.Name = name)
        if Seq.isEmpty nodes || Seq.length nodes <> 1 then
            None
        else
            Some <| Seq.head nodes

    /// Searches for a given association in a given model by target name and additional predicate. Assumes that it
    /// exists and there is only one such association. Throws InvalidSemanticOperationException if not.
    let private findAssociationIn (edges: IEdge seq) targetName =
        let associations =
            edges
            |> Seq.filter
                (fun e -> e :? IAssociation && (e :?> IAssociation).TargetName = targetName
                )

        if Seq.isEmpty associations then
            raise (InvalidSemanticOperationException <| sprintf "Edge %s not found" targetName)
        elif Seq.length associations <> 1 then
            raise (InvalidSemanticOperationException
                <| sprintf "Edge %s appears more than once" targetName)
        else
            Seq.head associations :?> IAssociation

    /// Searches for a given association in a given model by target name. Assumes that it exists and there is only one
    /// association with that name. Throws InvalidSemanticOperationException if not.
    let findAssociation (model: IModel) targetName =
        findAssociationIn model.Edges targetName

    /// Searches for a given association starting in a given element with a given name. Assumes that it exists and
    /// there is only one association with that name. Throws InvalidSemanticOperationException if not.
    let findAssociationWithSource (element: IElement) targetName =
        findAssociationIn element.OutgoingEdges targetName
