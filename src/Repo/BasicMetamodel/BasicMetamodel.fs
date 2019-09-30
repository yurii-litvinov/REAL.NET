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
type IBasicMetamodelElement =
    interface
        /// Outgoing edges for that element.
        abstract OutgoingEdges: IBasicMetamodelEdge seq with get
    end

/// Node is a kind of element which can connect only to edge, corresponds to node of the model graph.
and IBasicMetamodelNode =
    interface
        inherit IBasicMetamodelElement

        /// Name of a node, possibly not unique.
        abstract Name: string with get, set
    end

/// Edge is a kind of element which can connect to everything.
and IBasicMetamodelEdge =
    interface
        inherit IBasicMetamodelElement

        /// Element at the beginning of an edge.
        abstract Source: IBasicMetamodelElement with get, set

        /// Element at the ending of an edge.
        abstract Target: IBasicMetamodelElement with get, set

        /// String describing a target of an edge. For example, field name in UML can be written on association
        /// next to target (type of the field).
        abstract TargetName: string with get, set
    end

/// Repository is a collection of elements.
type IBasicMetamodelRepository =
    interface
        /// Creates a new node in a repository.
        abstract CreateNode: name: string -> IBasicMetamodelNode

        /// Creates new edge with given source, target and target name. 
        abstract CreateEdge: 
                source: IBasicMetamodelElement ->
                target: IBasicMetamodelElement ->
                targetName: string
                -> IBasicMetamodelEdge

        /// Returns all elements in a repository.
        abstract Elements: IBasicMetamodelElement seq with get

        /// Returns all nodes in a repository.
        abstract Nodes: IBasicMetamodelNode seq with get

        /// Returns all edges in a repository.
        abstract Edges: IBasicMetamodelEdge seq with get

        /// Deletes element from a repository. Removes "hanging" edges. Nodes without connections are not removed 
        /// automatically.
        abstract DeleteElement: element : IBasicMetamodelElement -> unit

        /// Searches node in a model. If there are none or more than one node with given name, throws an exception.
        abstract Node: name: string -> IBasicMetamodelNode

        /// Returns true if a node with given name exists in a model.
        abstract HasNode: name: string -> bool

        /// Searches edge with given target name in a model. If there are none or more than one association 
        /// with given name, throws an exception.
        abstract Edge: name: string -> IBasicMetamodelEdge

        /// Returns true if an edge with given target name exists in a model.
        abstract HasEdge: name: string -> bool

            /// Clears repository contents.
        abstract Clear : unit -> unit
    end
