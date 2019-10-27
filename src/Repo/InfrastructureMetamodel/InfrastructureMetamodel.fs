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

namespace Repo.InfrastructureMetamodel

/// Element, most general thing that can be in a model.
type IInfrastructureElement =
    interface
        /// Outgoing associations for that element.
        abstract OutgoingAssociations: IInfrastructureAssociation seq with get

        /// Incoming associations for that element.
        abstract IncomingAssociations: IInfrastructureAssociation seq with get

        /// Direct parents of this element in generalization hierarchy.
        abstract DirectSupertypes : IInfrastructureElement seq with get

        /// A list of all attributes available for an element.
        abstract Attributes: IInfrastructureAttribute seq with get

        /// A list of all slots for an element.
        abstract Slots: IInfrastructureSlot seq with get

        /// Returns a model to which this element belongs.
        abstract Model: IInfrastructureModel with get

        /// False when metatype of an element can not be represented in terms of Attribute Metamodel.
        /// InstanceOf edges always have this property set to false, to avoid infinite recursion.
        abstract HasMetatype: bool with get

        /// Returns an element that this element is an instance of (target of an "instanceOf" association).
        abstract Metatype: IInfrastructureElement with get
    end

/// Attribute is like field in a class --- describes possible values of a field in instances. Has type 
/// (a set of possible values), name and default value. Since default value can be an ontological instance of type,
/// type can be of a higher metalevel (so "Type" association can cross metalayer boundaries).
///
/// In theory, DefaultValue in not an actual value, but a method to produce one in model instance, so we can avoid
/// metalayer crossing in some situations (for example, enum elements are nice ways to represent enum values).
///
/// For reference attributes (i.e. attributes that have a class from the same model as a type) there is no way to 
/// properly define default value other than something like null. If needed, "Type-object" pattern may be used
/// in lower metalayer languages to avoid such problems and to achieve behavior of textual languages.
and IInfrastructureAttribute =
    interface
        /// A type of an attribute. Restricts a set of possible values for corresponding slot.
        abstract Type: IInfrastructureElement with get

        /// A name of an attribute.
        abstract Name: string with get

        /// Default value for an attribute. Used when no value for an attribute is given during instantiation.
        abstract DefaultValue: IInfrastructureElement with get
    end

/// An instance of attribute. Contains actual value.
and IInfrastructureSlot =
    interface
        /// Attribute that this slot is an instance of.
        abstract Attribute: IInfrastructureAttribute with get

        /// Value of a slot.
        abstract Value: IInfrastructureElement with get, set
    end

/// Node is a kind of element which can connect only to edge, corresponds to node of the model graph.
/// NOTE: Node can be seen as always unconnected edge.
and IInfrastructureNode =
    interface
        inherit IInfrastructureElement

        /// Name of a node, possibly not unique.
        abstract Name: string with get, set
    end

/// Enumeration is a custom data type that consists of some named elements.
and IInfrastructureEnumeration =
    interface
        inherit IInfrastructureElement

        /// Name of an enumeration.
        abstract Name: string with get, set

        /// List of elements belonging to the enumeration.
        abstract Elements: IInfrastructureNode seq with get

        /// Adds a new enumeration element.
        abstract AddElement: name: string -> unit
    end

/// Edge is a kind of element which can connect to everything.
and IInfrastructureEdge =
    interface
        inherit IInfrastructureElement
        /// Element at the beginning of an edge, may be None if edge is not connected.
        abstract Source: IInfrastructureElement with get, set

        /// Element at the ending of an edge, may be None if edge is not connected.
        abstract Target: IInfrastructureElement with get, set
    end

/// Generalization is a kind of edge which has special semantic in metamodel (allows to inherit associations).
and IInfrastructureGeneralization =
    interface
        inherit IInfrastructureEdge
    end

/// Association is a general kind of edge, has string attribute describing target of an edge.
and IInfrastructureAssociation =
    interface
        inherit IInfrastructureEdge

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
and IInfrastructureInstanceOf =
    interface
        inherit IInfrastructureEdge
    end

/// Model is a set of nodes and edges, corresponds to one diagram (or one palette) in editor.
and IInfrastructureModel =
    interface
        /// Model can have descriptive name (must be unique).
        abstract Name: string with get, set

        /// Returns true if metamodel of this model is representable in Attribute Metamodel semantics.
        abstract HasMetamodel: bool with get

        /// Metamodel is a model whose elements are types of elements for this model.
        /// Model can be a metamodel for itself.
        abstract Metamodel: IInfrastructureModel with get

        /// Creates a new node in a model by instantiating Attribute Metamodel "Node".
        abstract CreateNode: name: string -> IInfrastructureNode

        /// Creates new Generalization edge with given source and target by instantiating 
        /// Attribute Metamodel "Generalization"
        abstract CreateGeneralization: 
            source: IInfrastructureElement 
            -> target: IInfrastructureElement 
            -> IInfrastructureGeneralization

        /// Creates new Association edge with given source and target by instantiating 
        /// Attribute Metamodel "Association".
        abstract CreateAssociation:
            source: IInfrastructureElement
            -> target: IInfrastructureElement
            -> targetName: string
            -> IInfrastructureAssociation

        /// Creates a new node in a model by instantiating given node from metamodel, supplying given values
        /// to its slots. Slots without values receive values from DefaultValue property of corresponding attribute.
        abstract InstantiateNode:
            name: string
            -> metatype: IInfrastructureNode
            -> attributeValues: Map<string, IInfrastructureElement>
            -> IInfrastructureNode

        /// Creates a new association in a model by instantiating given association from metamodel.
        abstract InstantiateAssociation: 
            source: IInfrastructureElement 
            -> target: IInfrastructureElement 
            -> metatype: IInfrastructureAssociation 
            -> attributeValues: Map<string, IInfrastructureElement>
            -> IInfrastructureAssociation

        /// Returns all elements in a model.
        abstract Elements: IInfrastructureElement seq with get

        /// Returns all nodes in a model.
        abstract Nodes: IInfrastructureNode seq with get

        /// Returns all edges in a model.
        abstract Edges: IInfrastructureEdge seq with get

        /// Deletes element from a model and unconnects related elements if needed. Removes "hanging" edges.
        /// Nodes without connections are not removed automatically.
        abstract DeleteElement: element : IInfrastructureElement -> unit

        /// Searches node in a model. If there are none or more than one node with given name, throws an exception.
        abstract Node: name: string -> IInfrastructureNode

        /// Returns true if a node with given name exists in a model.
        abstract HasNode: name: string -> bool

        /// Searches association with given traget name in a model. If there are none or more than one association 
        /// with given name, throws an exception.
        abstract Association: name: string -> IInfrastructureAssociation

        /// Returns true if an association with given target name exists in a model.
        abstract HasAssociation: name: string -> bool

        /// Prints model contents on a console.
        abstract PrintContents: unit -> unit
    end

/// Repository is a collection of models.
type IInfrastructureRepository =
    interface
        /// All models in a repository.
        abstract Models: IInfrastructureModel seq with get

        /// Creates and returns a new model in repository based on Attribute Metamodel.
        abstract InstantiateAttributeMetamodel: name: string -> IInfrastructureModel

        /// Creates and returns a new model in repository based on a given metamodel.
        abstract InstantiateModel: name: string -> metamodel: IInfrastructureModel -> IInfrastructureModel

        /// Searches model in repository. 
        /// If there are none or more than one model with given name, throws an exception.
        abstract Model: name: string -> IInfrastructureModel

        /// Deletes given model and all its elements from repository.
        abstract DeleteModel: model : IInfrastructureModel -> unit

        /// Clears repository contents.
        abstract Clear : unit -> unit
    end
