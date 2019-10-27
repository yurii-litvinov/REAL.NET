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

namespace Repo.LanguageMetamodel

/// Element, most general thing that can be in a model.
type ILanguageElement =
    interface
        /// Outgoing associations for that element.
        abstract OutgoingAssociations: ILanguageAssociation seq with get

        /// Returns outgoing association with a given target name if there is exactly one such edge. 
        /// Throws ElementNotFoundException if there is no edge with such name and MultipleElementsException 
        /// if there are many.
        abstract OutgoingAssociation: name: string -> ILanguageAssociation

        /// Incoming associations for that element.
        abstract IncomingAssociations: ILanguageAssociation seq with get

        /// Direct parents of this element in generalization hierarchy.
        abstract DirectSupertypes : ILanguageElement seq with get

        /// A list of all attributes available for an element.
        abstract Attributes: ILanguageAttribute seq with get

        /// A list of all slots for an element.
        abstract Slots: ILanguageSlot seq with get

        /// Returns a model to which this element belongs.
        abstract Model: ILanguageModel with get

        /// False when metatype of an element can not be represented in terms of Attribute Metamodel.
        /// InstanceOf edges always have this property set to false, to avoid infinite recursion.
        abstract HasMetatype: bool with get

        /// Returns an element that this element is an instance of (target of an "instanceOf" association).
        abstract Metatype: ILanguageElement with get
    end

/// Attribute is like field in a class --- describes possible values of a field in instances. Has type 
/// (a set of possible values) and name.
and ILanguageAttribute =
    interface
        /// A type of an attribute. Restricts a set of possible values for corresponding slot.
        abstract Type: ILanguageElement with get

        /// A name of an attribute.
        abstract Name: string with get
    end

/// An instance of attribute. Contains actual value.
and ILanguageSlot =
    interface
        /// Attribute that this slot is an instance of.
        abstract Attribute: ILanguageAttribute with get

        /// Value of a slot.
        abstract Value: ILanguageElement with get, set
    end

/// Node is a kind of element which can connect only to edge, corresponds to node of the model graph.
and ILanguageNode =
    interface
        inherit ILanguageElement

        /// Name of a node, possibly not unique.
        abstract Name: string with get, set
    end

/// Enumeration is a custom data type that consists of some named elements.
and ILanguageEnumeration =
    interface
        inherit ILanguageElement

        /// Name of an enumeration.
        abstract Name: string with get, set

        /// List of elements belonging to the enumeration.
        abstract Elements: ILanguageNode seq with get

        /// Adds a new enumeration element.
        abstract AddElement: name: string -> unit
    end

/// Edge is a kind of element which can connect to everything.
and ILanguageEdge =
    interface
        inherit ILanguageElement
        /// Element at the beginning of an edge, may be None if edge is not connected.
        abstract Source: ILanguageElement with get, set

        /// Element at the ending of an edge, may be None if edge is not connected.
        abstract Target: ILanguageElement with get, set
    end

/// Generalization is a kind of edge which has special semantic in metamodel (allows to inherit associations).
and ILanguageGeneralization =
    interface
        inherit ILanguageEdge
    end

/// Association is a general kind of edge, has string attribute describing target of an edge.
and ILanguageAssociation =
    interface
        inherit ILanguageEdge

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
and ILanguageInstanceOf =
    interface
        inherit ILanguageEdge
    end

/// Model is a set of nodes and edges, corresponds to one diagram (or one palette) in editor.
and ILanguageModel =
    interface
        /// Model can have descriptive name (must be unique).
        abstract Name: string with get, set

        /// Returns true if metamodel of this model is representable in Attribute Metamodel semantics.
        abstract HasMetamodel: bool with get

        /// Metamodel is a model whose elements are types of elements for this model.
        /// Model can be a metamodel for itself.
        abstract Metamodel: ILanguageModel with get

        /// Creates a new node in a model by instantiating Attribute Metamodel "Node".
        abstract CreateNode: name: string -> ILanguageNode

        /// Creates new Generalization edge with given source and target by instantiating 
        /// Attribute Metamodel "Generalization"
        abstract CreateGeneralization: 
            source: ILanguageElement 
            -> target: ILanguageElement 
            -> ILanguageGeneralization

        /// Creates new Association edge with given source and target by instantiating 
        /// Attribute Metamodel "Association".
        abstract CreateAssociation:
            source: ILanguageElement
            -> target: ILanguageElement
            -> targetName: string
            -> ILanguageAssociation

        /// Creates a new node in a model by instantiating given node from metamodel, supplying given values
        /// to its slots. Slots without values receive values from DefaultValue property of corresponding attribute.
        abstract InstantiateNode:
            name: string
            -> metatype: ILanguageNode
            -> attributeValues: Map<string, ILanguageElement>
            -> ILanguageNode

        /// Creates a new association in a model by instantiating given association from metamodel.
        abstract InstantiateAssociation: 
            source: ILanguageElement 
            -> target: ILanguageElement 
            -> metatype: ILanguageAssociation 
            -> attributeValues: Map<string, ILanguageElement>
            -> ILanguageAssociation

        /// Creates a new enumeration in a model.
        abstract CreateEnumeration: name: string -> elements: string seq -> ILanguageEnumeration

        /// Returns all elements in a model.
        abstract Elements: ILanguageElement seq with get

        /// Returns all nodes in a model.
        abstract Nodes: ILanguageNode seq with get

        /// Returns all edges in a model.
        abstract Edges: ILanguageEdge seq with get

        /// Deletes element from a model and unconnects related elements if needed. Removes "hanging" edges.
        /// Nodes without connections are not removed automatically.
        abstract DeleteElement: element : ILanguageElement -> unit

        /// Searches node in a model. If there are none or more than one node with given name, throws an exception.
        abstract Node: name: string -> ILanguageNode

        /// Returns true if a node with given name exists in a model.
        abstract HasNode: name: string -> bool

        /// Searches association with given traget name in a model. If there are none or more than one association 
        /// with given name, throws an exception.
        abstract Association: name: string -> ILanguageAssociation

        /// Returns true if an association with given target name exists in a model.
        abstract HasAssociation: name: string -> bool

        /// Prints model contents on a console.
        abstract PrintContents: unit -> unit
    end

/// Repository is a collection of models.
type ILanguageRepository =
    interface
        /// All models in a repository.
        abstract Models: ILanguageModel seq with get

        /// Creates and returns a new model in repository based on Language Metamodel.
        abstract InstantiateLanguageMetamodel: name: string -> ILanguageModel

        /// Creates and returns a new model in repository based on a given metamodel.
        abstract InstantiateModel: name: string -> metamodel: ILanguageModel -> ILanguageModel

        /// Searches model in repository. 
        /// If there are none or more than one model with given name, throws an exception.
        abstract Model: name: string -> ILanguageModel

        /// Deletes given model and all its elements from repository.
        abstract DeleteModel: model : ILanguageModel -> unit

        /// Clears repository contents.
        abstract Clear : unit -> unit
    end
