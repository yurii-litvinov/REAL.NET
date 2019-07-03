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
type IDataElement =
    interface
        /// Type of an element.
        abstract Class: IDataElement with get

        /// Outgoing edges for that element.
        abstract OutgoingEdges: IDataEdge seq with get

        /// Incoming edges for that element.
        abstract IncomingEdges: IDataEdge seq with get

        /// Adds outgoing edge to this element.
        abstract AddOutgoingEdge: edge: IDataEdge -> unit

        /// Adds incoming edge to this element.
        abstract AddIncomingEdge: edge: IDataEdge -> unit

        /// Deletes outgoing edge from this element.
        abstract DeleteOutgoingEdge: edge: IDataEdge -> unit

        /// Deletes incoming edge from this element.
        abstract DeleteIncomingEdge: edge: IDataEdge -> unit

        /// Returns a model to which this element belongs.
        abstract Model: IDataModel with get
    end

/// Node is a kind of element which can connect only to edge, corresponds to node of the model graph.
/// NOTE: Node can be seen as always unconnected edge.
and IDataNode =
    interface
        inherit IDataElement

        /// Name of a node, possibly not unique.
        abstract Name: string with get, set
    end

/// Edge is a kind of element which can connect to everything.
and IDataEdge =
    interface
        inherit IDataElement
        /// Element at the beginning of an edge, may be None if edge is not connected.
        abstract Source: IDataElement option with get, set

        /// Element at the ending of an edge, may be None if edge is not connected.
        abstract Target: IDataElement option with get, set
    end

/// Generalization is a kind of edge which has special semantic in metamodel (allows to inherit associations).
and IDataGeneralization =
    interface
        inherit IDataEdge
    end

/// Association is a general kind of edge, has string attribute describing target of an edge.
and IDataAssociation =
    interface
        inherit IDataEdge

        /// String describing a target of an association. For example, field name in UML can be written on association
        /// next to target (type of the field).
        abstract TargetName: string with get, set
    end

/// Model is a set of nodes and edges, corresponds to one diagram (or one palette) in editor.
and IDataModel =
    interface
        /// Model can have descriptive name (must be unique).
        abstract Name: string with get, set

        /// Metamodel is a model whose elements are types of elements for this model. Model can be a metamodel
        /// for itself.
        abstract Metamodel: IDataModel with get

        /// Creates a new node of given class in a model.
        abstract CreateNode: name: string * ``class``: IDataElement -> IDataNode

        /// Creates a node that is its own type (Node, for example, is an instance of Node).
        abstract CreateNode: name: string -> IDataNode

        /// Creates new Generalization edge with given source and target.
        abstract CreateGeneralization: ``class``: IDataElement * source: IDataElement * target: IDataElement -> IDataGeneralization

        /// Creates new possibly unconnected Generalization edge.
        abstract CreateGeneralization: ``class``: IDataElement * source: IDataElement option * target: IDataElement option -> IDataGeneralization

        /// Creates new Association edge with given source and target.
        abstract CreateAssociation:
                ``class``: IDataElement
                * source: IDataElement
                * target: IDataElement
                * targetName: string
                -> IDataAssociation

        /// Creates new possibly unconnected Association edge.
        abstract CreateAssociation:
                ``class``: IDataElement
                * source: IDataElement option
                * target: IDataElement option
                * targetName: string
                -> IDataAssociation

        /// Returns all elements in a model.
        abstract Elements: IDataElement seq with get

        /// Returns all nodes in a model.
        abstract Nodes: IDataNode seq with get

        /// Returns all edges in a model.
        abstract Edges: IDataEdge seq with get

        /// A map of custom model properties.
        abstract Properties: Map<string, string> with get, set

        /// Deletes element from a model and unconnects related elements if needed.
        abstract DeleteElement: element : IDataElement -> unit

        /// Searches node in a model. If there are none or more than one node with given name, throws an exception.
        abstract Node: name: string -> IDataNode

        /// Returns true if a node with given name exists in a model.
        abstract HasNode: name: string -> bool

    end

/// Repository is a collection of models.
type IDataRepository =
    interface
        /// All models in a repository
        abstract Models: IDataModel seq with get

        /// Creates and returns a new model in repository.
        abstract CreateModel: name : string -> IDataModel

        /// Creates and returns a new model in repository based on a given metamodel.
        abstract CreateModel: name: string * metamodel: IDataModel -> IDataModel

        /// Deletes given model from repository.
        abstract DeleteModel: model : IDataModel -> unit

        /// Clears repository contents.
        abstract Clear : unit -> unit

        /// Searches model in repository. If there are none or more than one model with given name, throws an exception.
        abstract Model: name: string -> IDataModel
    end
