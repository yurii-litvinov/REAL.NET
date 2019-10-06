(* Copyright 2019 REAL.NET group
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

namespace Repo.BasicMetamodel

/// Element, most general thing that can be in a model.
type IBasicElement =
    interface
        /// Outgoing edges for that element.
        abstract OutgoingEdges: IBasicEdge seq with get

        /// Searches outgoing edge by target name. Throws ElementNotFoundException if there is no outgoing edge with 
        /// this target name, MultipleElementsException if there are many.
        abstract OutgoingEdge: name: string -> IBasicEdge

        /// Checks that exactly one outgoing edge with this name exists
        abstract HasExactlyOneOutgoingEdge: name: string -> bool

        /// Returns all elements that this element is an instance of (targets of all "instanceOf" associations).
        abstract Metatypes: IBasicElement seq with get

        /// Returns an element that this element is an instance of (target of an "instanceOf" association),
        /// if this element is exactly one. Throws ElementNotFoundException if there is no metatype and
        /// MultipleElementsException if there are many.
        abstract Metatype: IBasicElement with get
    end

/// Node is a kind of element which can connect only to edge, corresponds to node of the model graph.
and IBasicNode =
    interface
        inherit IBasicElement

        /// Name of a node, possibly not unique.
        abstract Name: string with get, set
    end

/// Edge is a kind of element which can connect to everything.
and IBasicEdge =
    interface
        inherit IBasicElement

        /// Element at the beginning of an edge.
        abstract Source: IBasicElement with get, set

        /// Element at the ending of an edge.
        abstract Target: IBasicElement with get, set

        /// String describing a target of an edge. For example, field name in UML can be written on association
        /// next to target (type of the field).
        abstract TargetName: string with get, set
    end

/// Repository is a collection of elements.
type IBasicRepository =
    interface
        /// Creates a new node in a repository.
        abstract CreateNode: name: string -> IBasicNode

        /// Creates new edge with given source, target and target name. 
        abstract CreateEdge: 
                source: IBasicElement ->
                target: IBasicElement ->
                targetName: string
                -> IBasicEdge

        /// Returns all elements in a repository.
        abstract Elements: IBasicElement seq with get

        /// Returns all nodes in a repository.
        abstract Nodes: IBasicNode seq with get

        /// Returns all edges in a repository.
        abstract Edges: IBasicEdge seq with get

        /// Deletes element from a repository. Removes "hanging" edges. Nodes without connections are not removed 
        /// automatically.
        abstract DeleteElement: element : IBasicElement -> unit

        /// Searches node in a model. If there are none or more than one node with given name, throws an exception.
        abstract Node: name: string -> IBasicNode

        /// Returns true if a node with given name exists in a model.
        abstract HasNode: name: string -> bool

        /// Searches edge with given target name in a model. If there are none or more than one association 
        /// with given name, throws an exception.
        abstract Edge: name: string -> IBasicEdge

        /// Returns true if an edge with given target name exists in a model.
        abstract HasEdge: name: string -> bool

        /// Clears repository contents.
        abstract Clear: unit -> unit
    end
