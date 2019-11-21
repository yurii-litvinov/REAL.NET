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

namespace Repo.DataLayer

/// Element, most general thing that can be in a model.
type IElement =
    interface
        /// Type of an element.
        abstract Class: IElement with get

        /// Outgoing edges for that element.
        abstract OutgoingEdges: IEdge seq with get

        /// Incoming edges for that element.
        abstract IncomingEdges: IEdge seq with get

        /// Adds outgoing edge to this element.
        abstract AddOutgoingEdge: edge: IEdge -> unit

        /// Adds incoming edge to this element.
        abstract AddIncomingEdge: edge: IEdge -> unit

        /// Deletes outgoing edge from this element.
        abstract DeleteOutgoingEdge: edge: IEdge -> unit

        /// Deletes incoming edge from this element.
        abstract DeleteIncomingEdge: edge: IEdge -> unit

        /// Returns a model to which this element belongs.
        abstract Model: IModel with get

        /// Is this element should be deleted or serialized
        abstract IsMarkedDeleted: bool with get, set
    end

/// Node is a kind of element which can connect only to edge, corresponds to node of the model graph.
/// NOTE: Node can be seen as always unconnected edge.
and INode =
    interface
        inherit IElement

        /// Name of a node, possibly not unique.
        abstract Name: string with get, set
    end

/// Edge is a kind of element which can connect to everything.
and IEdge =
    interface
        inherit IElement
        /// Element at the beginning of an edge, may be None if edge is not connected.
        abstract Source: IElement option with get, set

        /// Element at the ending of an edge, may be None if edge is not connected.
        abstract Target: IElement option with get, set
    end

/// Generalization is a kind of edge which has special semantic in metamodel (allows to inherit associations).
and IGeneralization =
    interface
        inherit IEdge
    end

/// Association is a general kind of edge, has string attribute describing target of an edge.
and IAssociation =
    interface
        inherit IEdge

        /// String describing a target of an association. For example, field name in UML can be written on association
        /// next to target (type of the field).
        abstract TargetName: string with get, set
    end

/// Model is a set of nodes and edges, corresponds to one diagram (or one palette) in editor.
and IModel =
    interface
        /// Model can have descriptive name (must be unique).
        abstract Name: string with get, set

        /// Metamodel is a model whose elements are types of elements for this model. Model can be a metamodel
        /// for itself.
        abstract Metamodel: IModel with get

        /// Creates a new node of given class in a model.
        abstract CreateNode: name: string * ``class``: IElement -> INode

        /// Creates a node that is its own type (Node, for example, is an instance of Node).
        abstract CreateNode: name: string * func: Option<IElement> -> INode

        /// Creates new Generalization edge with given source and target.
        abstract CreateGeneralization: ``class``: IElement * source: IElement * target: IElement -> IGeneralization

        /// Creates new possibly unconnected Generalization edge.
        abstract CreateGeneralization: ``class``: IElement * source: IElement option * target: IElement option -> IGeneralization

        /// Creates new Association edge with given source and target.
        abstract CreateAssociation:
                ``class``: IElement
                * source: IElement
                * target: IElement
                * targetName: string
                -> IAssociation

        /// Creates new possibly unconnected Association edge.
        abstract CreateAssociation:
                ``class``: IElement
                * source: IElement option
                * target: IElement option
                * targetName: string
                -> IAssociation

        /// Returns all elements in a model.
        abstract Elements: IElement seq with get

        /// Returns all nodes in a model.
        abstract Nodes: INode seq with get

        /// Returns all edges in a model.
        abstract Edges: IEdge seq with get

        /// A map of custom model properties.
        abstract Properties: Map<string, string> with get, set

        /// Deletes element from a model and unconnects related elements if needed.
        abstract MarkElementDeleted: element : IElement -> unit

        /// Restores element after deleting.
        abstract UnmarkDeletedElement: element : IElement -> unit
    end

/// Repository is a collection of models.
type IRepo =
    interface
        /// All models in a repository
        abstract Models: IModel seq with get

        /// Creates and returns a new model in repository.
        abstract CreateModel: name : string -> IModel

        /// Creates and returns a new model in repository based on a given metamodel.
        abstract CreateModel: name: string * metamodel: IModel -> IModel

        /// Deletes given model from repository.
        abstract DeleteModel: model : IModel -> unit

        /// Clears repository contents.
        abstract Clear : unit -> unit
    end
