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

open Repo.CoreMetamodel
open Repo.BasicMetamodel

/// Implementation of Element.
[<AbstractClass>]
type CoreElement(element: IBasicElement, pool: CorePool, repo: IBasicRepository) =

    let isInstanceOf metatype (element: IBasicElement) =
        element.Metatype = metatype

    let hasOutgoingEdgeTo (target: IBasicElement) element =
        target.OutgoingEdges 
        |> Seq.filter (fun e -> e.Target = element)
        |> Seq.isEmpty
        |> not

    member this.UnderlyingElement = element

    interface ICoreElement with
        member this.OutgoingEdges =
            element.OutgoingEdges
            |> Seq.map pool.Wrap
            |> Seq.cast<ICoreEdge>

        member this.IncomingEdges =
            repo.Edges 
            |> Seq.filter (fun e -> e.Target = element)
            |> Seq.map pool.Wrap
            |> Seq.cast<ICoreEdge>

        member this.IsContainedInSomeModel =
            let modelMetatype = repo.Node Consts.metamodelModel

            repo.Nodes
            |> Seq.filter (isInstanceOf modelMetatype)
            |> Seq.filter (hasOutgoingEdgeTo element)
            |> Seq.isEmpty
            |> not

        member this.Model: ICoreModel =
            let modelMetatype = repo.Node Consts.metamodelModel

            repo.Nodes
            |> Seq.filter (isInstanceOf modelMetatype)
            |> Seq.filter (hasOutgoingEdgeTo element)
            |> Seq.exactlyOne
            |> pool.WrapModel

        member this.Metatypes =
            element.Metatypes
            |> Seq.map pool.Wrap

        member this.Metatype =
            pool.Wrap element.Metatype
