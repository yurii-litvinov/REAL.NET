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
    end

/// Node is a kind of element which can connect only to relationships, corresponds to node of the model graph.
/// NOTE: Node is an unconnected relationship, so it is highly possible that it does not have to be a separate class.
type INode =
    interface
        inherit IElement

        /// Name of a node, possibly not unique.
        abstract Name : string with get, set
    end

/// Relationship is a kind of element which can connect to everything, corresponds to edge of the model graph.
type IRelationship =
    interface
        inherit IElement
        /// Element at the beginning of relationship, may be None if relationship is not connected.
        abstract Source : IElement option with get, set

        /// Element at the ending of relationship, may be None if relationship is not connected.
        abstract Target : IElement option with get, set
    end

/// Generalization is a kind of relationship which has special semantic in metamodel (allows to inherit relations).
type IGeneralization =
    interface
        inherit IRelationship
    end

/// Association is a general kind of relationship, has string attribute describing target of relationship.
type IAssociation =
    interface
        inherit IRelationship
        abstract TargetName : string with get, set
    end

[<AllowNullLiteral>]
/// Model is a set of nodes and relationships, corresponds to one diagram (or one palette) in editor.
type IModel =
    interface
        /// Model can have descriptive name (possibly not unique).
        abstract Name: string with get, set

        /// Metamodel is a model whose elements are types of elements for this model. Model can be a metamodel 
        /// for itself.
        abstract Metamodel: IModel with get

        /// Creates a new node of given class in a model.
        abstract CreateNode: name: string * ``class``: IElement -> INode

        /// Creates a node that is its own type (Node, for example, is an instance of Node).
        abstract CreateNode: name: string -> INode

        /// Creates new Generalization relation with given source and target.
        abstract CreateGeneralization: ``class``: IElement * source: IElement * target: IElement -> IGeneralization

        /// Creates new possibly unconnected Generalization relation.
        abstract CreateGeneralization: ``class``: IElement * source: IElement option * target: IElement option -> IGeneralization

        /// Creates new Association relation with given source and target.
        abstract CreateAssociation: 
                ``class``: IElement 
                * source: IElement 
                * target: IElement 
                * targetName: string 
                -> IAssociation

        /// Creates new possibly unconnected Association relation.
        abstract CreateAssociation: 
                ``class``: IElement 
                * source: IElement option 
                * target: IElement option 
                * targetName: string 
                -> IAssociation

        /// Returns all elements in a model.
        abstract Elements : IElement seq with get

        /// Returns all nodes in a model.
        abstract Nodes : INode seq with get
        
        /// Returns all edges in a model.
        abstract Edges : IRelationship seq with get

        /// Deletes element from a model and unconnects related elements if needed.
        abstract DeleteElement : element : IElement -> unit
    end

/// Repository is a collection of related models.
type IRepo =
    interface
        /// All models in a repository
        abstract Models: IModel seq with get

        /// Creates and returns a new model in repository.
        abstract CreateModel: name : string -> IModel

        /// Creates and returns a new model in repository based on a given metamodel.
        abstract CreateModel: name: string * metamodel: IModel -> IModel

        /// Deletes given model from repository.
        abstract DeleteModel : model : IModel -> unit
    end
