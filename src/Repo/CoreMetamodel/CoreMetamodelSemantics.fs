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

namespace Repo.CoreMetamodel

open Repo
open Repo.DataLayer
open CoreSemanticsHelpers

/// Helper functions for element semantics.
type ElementSemantics () =

    /// Returns true if 'descendant' is a (possibly indirect) descendant of a 'parent', in terms of generalization
    /// hierarchy.
    static member IsDescendantOf (parent: IDataElement) (descendant: IDataElement) =
        bfs descendant isGeneralization ((=) parent) |> Option.isSome

    /// Returns true if an 'instance' is a (possibly indirect) instance of a 'class'.
    static member IsOntologicalInstanceOf (ontologicalClass: IDataElement) (instance: IDataElement) =
        if instance.OntologicalType = ontologicalClass 
                || ElementSemantics.IsDescendantOf ontologicalClass instance.OntologicalType 
        then
            true
        elif instance.OntologicalType = instance then
            false
        else
            ElementSemantics.IsOntologicalInstanceOf ontologicalClass instance.OntologicalType

    /// Returns all outgoing generalizations for an element.
    static member OutgoingGeneralizations element =
        ElementSemantics.OutgoingEdges element |> Seq.filter isGeneralization |> Seq.cast<IDataGeneralization>

    /// Returns all outgoing associations for an element.
    static member OutgoingAssociations element =
        ElementSemantics.OutgoingEdges element |> Seq.filter isAssociation |> Seq.cast<IDataAssociation>

    /// Returns outgoing association with given name.
    static member OutgoingAssociation (element: IDataElement) name =
        Helpers.getExactlyOne
            (ElementSemantics.OutgoingAssociations element)
            (fun (a: IDataAssociation) -> a.TargetName = name)
            (fun () -> InvalidSemanticOperationException <| sprintf "Association %s not found" name)
            (fun () -> InvalidSemanticOperationException <| sprintf "Association %s appears more than once" name)

    /// Returns true if outgoing association with given name exists for a given node.
    static member HasOutgoingAssociation (element: IDataElement) name =
        element |> ElementSemantics.OutgoingAssociations |> Seq.filter (fun a -> a.TargetName = name) |> Seq.isEmpty |> not

    /// Returns a model containing given element.
    static member ContainingModel (element: IDataElement) =
        element.Model

    /// Returns all outgoing edges for an element.
    static member OutgoingEdges (element: IDataElement) =
        element.OutgoingEdges

    /// Returns a sequence of all parents (in terms of generalization hierarchy) for given element.
    /// Most special node is the first in resulting sequence, most general is the last.
    static member Parents element =
        ElementSemantics.OutgoingGeneralizations element
        |> Seq.map (fun g -> g.Target)
        |> Seq.choose id
        |> Seq.map (fun p -> Seq.append (Seq.singleton p) (ElementSemantics.Parents p))
        |> Seq.concat

/// Helper functions for node semantics.
type NodeSemantics () =
    /// Returns name of a node.
    /// Throws InvalidSemanticOperationException if given element is not node so it does not have a name.
    static member Name (element: IDataElement) =
        if not <| element :? IDataNode then
            raise (InvalidSemanticOperationException "Only nodes have a name in REAL.NET")
        (element :?> IDataNode).Name

    /// Sets name of a node.
    /// Throws InvalidSemanticOperationException if given element is not node so it does not have a name.
    static member SetName name (element: IDataElement) =
        if not <| element :? IDataNode then
            raise (InvalidSemanticOperationException "Only nodes have a name in REAL.NET")
        (element :?> IDataNode).Name <- name
    
    /// Returns string representation of a node.
    static member ToString (node: IDataNode) =
        let result = sprintf "Name: %s\n" <| node.Name
        let result = result + (sprintf "Ontological type: %s\n" <| NodeSemantics.Name node.OntologicalType)
        let result = result + (sprintf "Linguistic type: %s\n" <| NodeSemantics.Name node.LinguisticType)
        result

/// Helper functions for working with models.
type ModelSemantics () =
    /// Searches for a given association in a given model by target name and additional predicate. Assumes that it
    /// exists and there is only one such association. Throws InvalidSemanticOperationException if not.
    static let FindAssociationIn (edges: IDataEdge seq) targetName =
        Helpers.getExactlyOne edges
                (fun e -> e :? IDataAssociation && (e :?> IDataAssociation).TargetName = targetName)
                (fun () -> InvalidSemanticOperationException 
                        <| sprintf "Edge %s not found" targetName)
                (fun () -> InvalidSemanticOperationException 
                        <| sprintf "Edge %s appears more than once" targetName)
        :?> IDataAssociation

    /// Searches for a given node in a given model by name, returns None if not found or found more than one.
    static member TryFindNode (model: IDataModel) name =
        if model.HasNode name then
            Some <| model.Node name
        else
            None

    /// Searches for a given association in a given model by target name. Assumes that it exists and there is only one
    /// association with that name. Throws InvalidSemanticOperationException if not.
    static member FindAssociation (model: IDataModel) targetName =
        FindAssociationIn model.Edges targetName

    /// Searches for a given association starting in a given element with a given name. Assumes that it exists and
    /// there is only one association with that name. Throws InvalidSemanticOperationException if not.
    static member FindAssociationWithSource (element: IDataElement) targetName =
        FindAssociationIn element.OutgoingEdges targetName

/// Helper class that provides semantic operations on models conforming to Core Metamodel.
type CoreMetamodelSemantics(repo: IDataRepository) =

    let coreMetamodel = repo.Model "CoreMetamodel"
    let node = coreMetamodel.Node "Node"
    let association = coreMetamodel.Node "Association"

    /// Instantiates given node into given model. As Core Metamodel does not have a notion of attributes, instantiation
    /// is straightforward. New instance of a given node is created, without associations.
    member this.InstantiateNode
            (model: IDataModel)
            (ontologicalType: IDataNode) =
        model.CreateNode("a" + ontologicalType.Name, ontologicalType, node)

    /// Instantiates given association into given model.
    member this.InstantiateAssociation 
            (model: IDataModel) 
            (source: IDataNode) 
            (target: IDataNode) 
            (ontologicalType: IDataAssociation) =
        model.CreateAssociation(ontologicalType, association, source, target, ontologicalType.TargetName)
