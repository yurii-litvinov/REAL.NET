//(* Copyright 2017-2019 Yurii Litvinov
//*
//* Licensed under the Apache License, Version 2.0 (the "License");
//* you may not use this file except in compliance with the License.
//* You may obtain a copy of the License at
//*
//*     http://www.apache.org/licenses/LICENSE-2.0
//*
//* Unless required by applicable law or agreed to in writing, software
//* distributed under the License is distributed on an "AS IS" BASIS,
//* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//* See the License for the specific language governing permissions and
//* limitations under the License. *)

//namespace Repo.CoreMetamodel


///// Element, most general thing that can be in a model.
//type ICoreElement =
//    interface
//        /// Outgoing edges for that element.
//        abstract OutgoingEdges: IDataEdge seq with get

//        /// Incoming edges for that element.
//        abstract IncomingEdges: IDataEdge seq with get

//        /// Adds outgoing edge to this element.
//        abstract AddOutgoingEdge: edge: IDataEdge -> unit

//        /// Adds incoming edge to this element.
//        abstract AddIncomingEdge: edge: IDataEdge -> unit

//        /// Deletes outgoing edge from this element.
//        abstract DeleteOutgoingEdge: edge: IDataEdge -> unit

//        /// Deletes incoming edge from this element.
//        abstract DeleteIncomingEdge: edge: IDataEdge -> unit

//        /// Returns a model to which this element belongs.
//        abstract Model: IDataModel with get
//    end

///// Node is a kind of element which can connect only to edge, corresponds to node of the model graph.
///// NOTE: Node can be seen as always unconnected edge.
//and ICoreNode =
//    interface
//        inherit IDataElement

//        /// Name of a node, possibly not unique.
//        abstract Name: string with get, set
//    end

///// Edge is a kind of element which can connect to everything.
//and ICoreEdge =
//    interface
//        inherit IDataElement
//        /// Element at the beginning of an edge, may be None if edge is not connected.
//        abstract Source: IDataElement with get, set

//        /// Element at the ending of an edge, may be None if edge is not connected.
//        abstract Target: IDataElement with get, set
//    end

///// Generalization is a kind of edge which has special semantic in metamodel (allows to inherit associations).
//and ICoreGeneralization =
//    interface
//        inherit IDataEdge
//    end

///// Association is a general kind of edge, has string attribute describing target of an edge.
//and ICoreAssociation =
//    interface
//        inherit IDataEdge

//        /// String describing a target of an association. For example, field name in UML can be written on association
//        /// next to target (type of the field).
//        abstract TargetName: string with get, set
//    end

///// InstanceOf is a kind of edge which has special semantic in entire metamodel stack. Means that source is an instance
///// of target. Every element shall have at least one such outgoing edge (except InstanceOf itself, for it we assume
///// that it is always an instance of InstanceOf node of a corresponding metamodel). Can cross metamodel boundaries. 
///// There can be several InstanceOf relations from the same element (for example, linguistic InstanceOf and ontological
///// InstanceOf), to differentiate between them subsequent metalevels can add attributes to this edge.
/////
///// InstanceOf type is determined by metalevel of edge source and is governed by its linguistic metamodel (or just
///// metamodel, if linguistic and ontological metamodels are not differentiated on that metalevel). Note that
///// all linguistic metamodels shall have InstanceOf node and fully support InstanceOf semantics.
//and ICoreInstanceOf =
//    interface
//        inherit IDataEdge
//    end

///// Model is a set of nodes and edges, corresponds to one diagram (or one palette) in editor.
//and ICoreModel =
//    interface
//        /// Model can have descriptive name (must be unique).
//        abstract Name: string with get, set

//        /// Ontological metamodel is a model whose elements are ontological types of elements for this model.
//        /// Model can be a metamodel for itself.
//        abstract OntologicalMetamodel: IDataModel with get

//        /// Linguistic metamodel is a model whose elements are linguistic types of elements for this model.
//        /// Model can be a metamodel for itself.
//        abstract LinguisticMetamodel: IDataModel with get

//        /// Creates a new node in a model.
//        abstract CreateNode: name: string -> IDataNode

//        /// Creates new Generalization edge with given source and target. 
//        abstract CreateGeneralization: 
//                source: IDataElement
//                * target: IDataElement
//                -> IDataGeneralization

//        /// Creates new Association edge with given source and target.
//        abstract CreateAssociation:
//                source: IDataElement
//                * target: IDataElement
//                * targetName: string
//                -> IDataAssociation

//        /// Returns all elements in a model.
//        abstract Elements: IDataElement seq with get

//        /// Returns all nodes in a model.
//        abstract Nodes: IDataNode seq with get

//        /// Returns all edges in a model.
//        abstract Edges: IDataEdge seq with get

//        /// A map of custom model properties.
//        abstract Properties: Map<string, string> with get, set

//        /// Deletes element from a model and unconnects related elements if needed. Removes "hanging" edges.
//        /// Nodes without connections are not removed automatically.
//        abstract DeleteElement: element : IDataElement -> unit

//        /// Searches node in a model. If there are none or more than one node with given name, throws an exception.
//        abstract Node: name: string -> IDataNode

//        /// Returns true if a node with given name exists in a model.
//        abstract HasNode: name: string -> bool

//        /// Searches association with given traget name in a model. If there are none or more than one association 
//        /// with given name, throws an exception.
//        abstract Association: name: string -> IDataAssociation

//        /// Returns true if an association with given target name exists in a model.
//        abstract HasAssociation: name: string -> bool

//        /// Prints model contents on a console.
//        abstract PrintContents: unit -> unit

//    end

///// Repository is a collection of models.
//type ICoreRepository =
//    interface
//        /// All models in a repository.
//        abstract Models: IDataModel seq with get

//        /// Creates and returns a new model in repository which is linguistic and ontological metamodel of itself.
//        abstract CreateModel: name : string -> IDataModel

//        /// Creates and returns a new model in repository based on a given metamodel.
//        abstract CreateModel: 
//                name: string 
//                * ontologicalMetamodel: IDataModel
//                * linguisticMetamodel: IDataModel 
//                -> IDataModel

//        /// Deletes given model from repository.
//        abstract DeleteModel: model : IDataModel -> unit

//        /// Clears repository contents.
//        abstract Clear : unit -> unit

//        /// Searches model in repository. 
//        /// If there are none or more than one model with given name, throws an exception.
//        abstract Model: name: string -> IDataModel
//    end
