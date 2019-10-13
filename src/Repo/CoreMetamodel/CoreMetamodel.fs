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

/// Element, most general thing that can be in a model.
type ICoreElement =
    interface
        /// Outgoing edges (all of possible kinds) for that element.
        abstract OutgoingEdges: ICoreEdge seq with get

        /// Outgoing associations for that element.
        abstract OutgoingAssociations: ICoreAssociation seq with get

        /// Returns outgoing association with a given target name if there is exactly one such edge. 
        /// Throws ElementNotFoundException if there is no edge with such name and MultipleElementsException 
        /// if there are many.
        abstract OutgoingAssociation: name: string -> ICoreAssociation

        /// Incoming edges (all of possible kinds) for that element.
        abstract IncomingEdges: ICoreEdge seq with get

        /// Incoming associations for that element.
        abstract IncomingAssociations: ICoreAssociation seq with get

        /// Returns incoming association with a given target name if there is exactly one such edge. 
        /// Throws ElementNotFoundException if there is no edge with such name and MultipleElementsException 
        /// if there are many.
        abstract IncomingAssociation: name: string -> ICoreAssociation

        /// Returns false if an element is not contained in any model (true for upper metalayers where there is no
        /// notion of model or for "instanceOf" or "elements" edges).
        abstract IsContainedInSomeModel: bool with get

        /// Returns a list of all parents of this element (direct or transitive). Element itself is not included.
        abstract Generalizations: ICoreElement seq with get

        /// Returns a model to which this element belongs.
        abstract Model: ICoreModel with get

        /// Returns all elements that this element is an instance of (targets of all "instanceOf" associations).
        abstract Metatypes: ICoreElement seq with get

        /// Returns an element that this element is an instance of (target of an "instanceOf" association),
        /// if this element is exactly one. Throws ElementNotFoundException if there is no metatype and
        /// MultipleElementsException if there are many.
        abstract Metatype: ICoreElement with get

        /// Returns true if this element is an instance of a given element (possibly, indirect, i.e. instance of 
        /// instance of element). Respects generalization hierarchhy.
        abstract IsInstanceOf: element: ICoreElement -> bool

    end

/// Node is a kind of element which can connect only to edge, corresponds to node of the model graph.
/// NOTE: Node can be seen as always unconnected edge.
and ICoreNode =
    interface
        inherit ICoreElement

        /// Name of a node, possibly not unique.
        abstract Name: string with get, set
    end

/// Edge is a kind of element which can connect to everything.
and ICoreEdge =
    interface
        inherit ICoreElement
        /// Element at the beginning of an edge, may be None if edge is not connected.
        abstract Source: ICoreElement with get, set

        /// Element at the ending of an edge, may be None if edge is not connected.
        abstract Target: ICoreElement with get, set
    end

/// Generalization is a kind of edge which has special semantic in metamodel (allows to inherit associations).
and ICoreGeneralization =
    interface
        inherit ICoreEdge
    end

/// Association is a general kind of edge, has string attribute describing target of an edge.
and ICoreAssociation =
    interface
        inherit ICoreEdge

        /// String describing a target of an association. For example, field name in UML can be written on association
        /// next to target (type of the field).
        abstract TargetName: string with get, set
    end

/// InstanceOf is a kind of edge which has special semantic in entire metamodel stack. Means that source is an instance
/// of target. Every element shall have at least one such outgoing edge (except InstanceOf itself, for it we assume
/// that it is always an instance of InstanceOf node of a corresponding metamodel). Can cross metamodel boundaries. 
/// There can be several InstanceOf relations from the same element (for example, linguistic InstanceOf and ontological
/// InstanceOf), to differentiate between them subsequent metalevels can add attributes to this edge.
///
/// InstanceOf type is determined by metalevel of edge source and is governed by its linguistic metamodel (or just
/// metamodel, if linguistic and ontological metamodels are not differentiated on that metalevel). Note that
/// all linguistic metamodels shall have InstanceOf node and fully support InstanceOf semantics.
and ICoreInstanceOf =
    interface
        inherit ICoreEdge
    end

/// Model is a set of nodes and edges, corresponds to one diagram (or one palette) in editor.
and ICoreModel =
    interface
        /// Model can have descriptive name (must be unique).
        abstract Name: string with get, set

        /// Metamodel is a model whose elements are types of elements for this model.
        /// Model can be a metamodel for itself.
        abstract Metamodel: ICoreModel with get

        /// Creates a new node in a model by instantiating Core Metamodel "Node".
        abstract CreateNode: name: string -> ICoreNode

        /// Creates new Generalization edge with given source and target by instantiating 
        /// Core Metamodel "Generalization"
        abstract CreateGeneralization: source: ICoreElement -> target: ICoreElement -> ICoreGeneralization

        /// Creates new Association edge with given source and target by instantiating Core Metamodel "Association".
        abstract CreateAssociation: 
            source: ICoreElement 
            -> target: ICoreElement 
            -> targetName: string 
            -> ICoreAssociation

        /// Creates a new node in a model by instantiating given node from metamodel.
        abstract InstantiateNode: name: string -> metatype: ICoreNode -> ICoreNode

        /// Creates a new association in a model by instantiating given association from metamodel.
        abstract InstantiateAssociation: 
            source: ICoreElement 
            -> target: ICoreElement 
            -> metatype: ICoreAssociation 
            -> ICoreAssociation

        /// Returns all elements in a model.
        abstract Elements: ICoreElement seq with get

        /// Returns all nodes in a model.
        abstract Nodes: ICoreNode seq with get

        /// Returns all edges in a model.
        abstract Edges: ICoreEdge seq with get

        /// Deletes element from a model and unconnects related elements if needed. Removes "hanging" edges.
        /// Nodes without connections are not removed automatically.
        abstract DeleteElement: element : ICoreElement -> unit

        /// Searches node in a model. If there are none or more than one node with given name, throws an exception.
        abstract Node: name: string -> ICoreNode

        /// Returns true if a node with given name exists in a model.
        abstract HasNode: name: string -> bool

        /// Searches association with given traget name in a model. If there are none or more than one association 
        /// with given name, throws an exception.
        abstract Association: name: string -> ICoreAssociation

        /// Returns true if an association with given target name exists in a model.
        abstract HasAssociation: name: string -> bool

        /// Prints model contents on a console.
        abstract PrintContents: unit -> unit
    end

/// Repository is a collection of models.
type ICoreRepository =
    interface
        /// All models in a repository.
        abstract Models: ICoreModel seq with get

        /// Creates and returns a new model in repository based on Core Metamodel.
        abstract InstantiateCoreMetamodel: name: string -> ICoreModel

        /// Creates and returns a new model in repository based on a given metamodel.
        abstract InstantiateModel: name: string -> metamodel: ICoreModel -> ICoreModel

        /// Searches model in repository. 
        /// If there are none or more than one model with given name, throws an exception.
        abstract Model: name: string -> ICoreModel

        /// Returns Core Metamodel.
        abstract CoreMetamodel: ICoreModel with get

        /// Deletes given model and all its elements from repository.
        abstract DeleteModel: model : ICoreModel -> unit

        /// Clears repository contents.
        abstract Clear : unit -> unit
    end
