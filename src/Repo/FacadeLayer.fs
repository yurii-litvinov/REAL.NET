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

namespace Repo

/// Type of file which represents visual information
type TypeOfVisual =
    /// Xml file
    | XML = 0

    /// Just image connected to element
    | Image = 1

    /// No file provided
    | NoFile = 2

/// This interface represents information about how element is shown on screen.
type IVisualInfo =
    interface
        /// Address to  file. 
        abstract LinkToFile : string with get, set
        
        /// Type of linked file.
        abstract Type : TypeOfVisual with get, set 
    end

/// This interface represents information about how node is shown on screen.
type IVisualNodeInfo =
    interface
        inherit IVisualInfo

        /// Position of node on screen.
        abstract Position : (int * int) option with get, set          
    end

// This interface represents information about how edge is shown on screen.
type IVisualEdgeInfo =
    interface
        inherit IVisualInfo

        /// Coordinates of routing points without ends.
        abstract RoutingPoints : (int * int) list with get, set
    end

/// Enumeration with all kinds of attributes supported by repository
type AttributeKind =
    /// Attribute whose value is a string.
    | String = 0

    /// Attribute whose value is an integer.
    | Int = 1

    /// Attribute whose value is a floating point number with double precision.
    | Double = 2

    /// Attribute whose value is a boolean.
    | Boolean = 3

    /// Complex attribute kind, actual enumeration shall be in metamodel for given model, and attribute shall reference
    /// enumeration type for editor to be able to show available enumeration values.
    /// NOTE: Enums are not supported in v1, so AttributeKind value shall never be Enum.
    | Enum = 4

    /// Complex attribute kind, reference to another node in the same (or some other) model. Attribute shall reference
    /// target node in tis case. Example of reference types usage is UML class diagram, where field of a class
    /// can be an object of another class defined on the same diagram. Note that in UML reference attributes and
    /// associations are the same thing from the point of view of abstract syntax (and our Core Metamodel does not
    /// have a notion of attribute for that reason, they are modelled as associations). But concrete syntax (and,
    /// consequently, editors) shall distinguish these two cases.
    /// NOTE: Reference types are not supported in v1, so AttributeKind value shall never be Reference.
    | Reference = 5

/// "Metatype" of the element, i.e. type of the type. For example, edge can have type "Generalization", which is an
/// instance of "Edge". This information can be obtained by looking up metamodel, but editors need it readily available
/// to know what to do with the element.
type Metatype =
    /// Element is a node in model.
    | Node = 0

    /// Element is an edge.
    | Edge = 1

/// Attribute of an element (with given value). Provides a way to query and modify attribute value.
type IAttribute =
    interface
        /// Name of the attribute.
        abstract Name: string with get

        /// Kind (or a metatype) of the attribute --- one of elementary types, reference type or enum type.
        abstract Kind: AttributeKind with get

        /// Shall this attribute pass on to instances or is it defined for a class only and is not available for
        /// instances (much like static fields in a class). True if instances receive a copy of this attribute on
        /// instantiation, false if instantiation skips this attribute.
        abstract IsInstantiable: bool with get

        /// Reference to a type of the attribute if it is complex attribute, null if this is elementary type attribute.
        /// NOTE: Complex attribute types are not supported in v1, so it is always null.
        /// Note that even in v1 Infrastructure Metamodel may contain elements for elementary types, in such case
        /// this property shall return corresponding element.
        abstract Type: IElement with get

        /// String representation of a value of an attribute if it is an attribute of basic type.
        // NOTE: Actually we need a whole hierarchy of attribute classes.
        abstract StringValue: string with get, set

        /// Holds a reference to an element which is a value of this attribute if this attribute has complex type.
        /// NOTE: Complex attribute values are not supported in v1, so it is always null.
        abstract ReferenceValue: IElement with get, set
    end

/// Element is a general term for nodes or edges.
and [<AllowNullLiteral>] IElement =
    interface
        /// Name of an element, to be displayed on a scene and in various menus. Does not need to be unique.
        abstract Name: string with get, set

        /// Returns type of the element.
        abstract Class: IElement with get

        /// A list of all logical attributes for that element.
        abstract Attributes: IAttribute seq with get

        /// True when this element can not be a type of other elements, so it shall not be shown in palette.
        abstract IsAbstract: bool with get

        /// Metatype of this element (is it node or edge).
        abstract Metatype: Metatype with get

        /// Metatype of the instances of this element, if it is not abstract. Edge can have only edges as instances,
        /// but nodes may become edges (for example, node "Edge" becomes edge after instantiation).
        abstract InstanceMetatype: Metatype with get

        /// A string containing information about how to draw this element. May be XML document or some other string
        // that provides required info. It can be editor-specific, like XAML document for WPF-based editor, that will
        /// not be useful in WinForms editor. Also it can have different formats and different meaning for nodes and
        /// edges.
        /// TODO: shapes are actually more complex structures than strings, and some uniform format for shape
        /// representation is needed. Can be postponed after v1.
        abstract Shape: string with get

        /// Adds an attribute to a given element. Name of the attribute, its type and default value shall be specified.
        abstract AddAttribute: name: string * kind: AttributeKind * defaultValue: string -> unit
    end

/// Node --- well, a node in a model.
type INode =
    interface
        inherit IElement

        /// Info how element should be represented on screen
        abstract member VisualInfo: IVisualNodeInfo with get, set
    end

/// Edge --- an edge in a model. Note that here it can connect not only nodes, but edges too. It is needed to model
/// relations between edges (like "instance of" relation between association on a class diagram and link on object
/// diagram).
type IEdge =
    interface
        inherit IElement

        /// Reference to an element connected to a beginning of an edge, null if no element is connected.
        abstract From: IElement with get, set

        /// Reference to an element connected to an end of an edge, null if no element is connected.
        abstract To: IElement with get, set

        /// Info how element should be represented on screen
        abstract member VisualInfo: IVisualEdgeInfo with get, set
    end

/// Model is one "graph", represented by one diagram on a scene. Has name, consists of nodes and edges.
/// NOTE: v1 does not support any notion of hierarchical decomposition, so models do not have notion of diagrams,
/// subdiagrams and relations between diagrams. Model is a diagram and is drawn as a whole on one scene.
type IModel =
    interface
        /// Name of a model.
        abstract Name: string with get, set

        /// Shall this model be visible to user and selectable by user. True by default, but some tools
        /// may want to create their own internal models in a repository.
        abstract IsVisible: bool with get, set

        /// Reference to a metamodel for this model (may be reference to itself, for example, for Core Metametamodel).
        /// Can not be changed after model is created.
        abstract Metamodel: IModel with get

        /// A list of nodes in this model.
        abstract Nodes: INode seq with get

        /// A list of edges in this model.
        abstract Edges: IEdge seq with get

        /// A list of elements in this model. Calculated as Nodes merged with Edges.
        abstract Elements: IElement seq with get

        /// Instantiates element of a given type and returns result of an instantiation. Type shall be an element from
        /// Metamodel for this model.
        abstract CreateElement: ``type``: IElement -> IElement

        /// Helper method that instantiates element by a type name. Type is looked up in a metamodel and if no such 
        /// element found, exception will be thrown.
        abstract CreateElement: typeName: string -> IElement

        /// Deletes given element from a model.
        abstract DeleteElement: element: IElement -> unit

        /// Restores given element to this model
        abstract RestoreElement: element: IElement -> unit

        /// Searches for an element with given name in a model. Throws if there is no such element or there is 
        /// more than one.
        abstract FindElement: name: string -> IElement
    end

/// Repository is a collection of models.
type IRepo =
    interface
        /// A list of models in this repository. Shall always contain at least Core Metametamodel, because model
        /// semantics are defined through its elements and all other elements of all other models shall be the
        /// instances (possibly indirect) of some elements of Core Metametamodel.
        abstract Models: IModel seq with get

        /// Returns a model by name or throws if no such model found in a repository.
        abstract Model: name: string -> IModel

        /// Creates a new model with given name based on a given metamodel.
        abstract CreateModel: name: string * metamodel: IModel -> IModel

        /// Creates a new model with given name based on a metamodel with given name, throws if such metamodel 
        /// does not exist.
        abstract CreateModel: name: string * metamodelName: string -> IModel

        /// Deletes given model. Throws if repo contains models dependent on this model.
        /// Throws DeletingUsedModel if there are other models dependent on this.
        abstract DeleteModel: model: IModel -> unit

        /// Saves repository content into a file.
        abstract Save: fileName: string -> unit
    end

