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

namespace Repo.CoreMetamodel.Details.Elements

open Repo
open Repo.CoreMetamodel

/// Implementation of model.
type CoreModel(node: BasicMetamodel.IBasicNode, pool: CorePool, repo: BasicMetamodel.IBasicRepository) =

    let unwrap (element: ICoreElement) = (element :?> CoreElement).UnderlyingElement

    interface ICoreModel with

        member this.Name 
            with get () = node.Name
            and set v = node.Name <- v

        member this.Metamodel =
            let metamodel = (node.OutgoingEdge Consts.metamodelEdge).Target
            pool.WrapModel metamodel

        member this.CreateNode name =
            let node = repo.CreateNode name
            pool.Wrap node :?> ICoreNode

        member this.CreateGeneralization source target =
            let elementMetatype = repo.Node Consts.metamodelElement
            let generalizationMetatype = elementMetatype.OutgoingEdge Consts.generalization
            let edge = repo.CreateEdge (unwrap source) (unwrap target) ""
            repo.CreateEdge edge generalizationMetatype Consts.instanceOfEdge |> ignore
            pool.Wrap edge :?> ICoreGeneralization

        member this.CreateAssociation source target targetName =
            let edgeMetatype = repo.Node "Edge"
            let edge = repo.CreateEdge (unwrap source) (unwrap target) ""
            repo.CreateEdge edge edgeMetatype "instanceOf" |> ignore
            pool.Wrap edge :?> ICoreAssociation

        member this.Elements =
            let modelMetatype = repo.Node Consts.metamodelModel
            let elementsMetatype = modelMetatype.OutgoingEdge Consts.elementsEdge :> BasicMetamodel.IBasicElement

            node.OutgoingEdges
            |> Seq.filter (fun e -> (e.Metatypes |> Seq.isEmpty |> not) && (e.Metatype = elementsMetatype))
            |> Seq.map (fun e -> e.Target)
            |> Seq.map pool.Wrap

        member this.Nodes =
            (this :> ICoreModel).Elements
            |> Seq.filter (fun e -> e :? ICoreNode)
            |> Seq.cast<ICoreNode>

        member this.Edges =
            (this :> ICoreModel).Elements
            |> Seq.filter (fun e -> e :? ICoreEdge)
            |> Seq.cast<ICoreEdge>

        (*
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
        *)