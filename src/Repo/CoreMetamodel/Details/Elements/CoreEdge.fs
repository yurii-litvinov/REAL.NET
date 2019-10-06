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

/// Implementation of Edge.
[<AbstractClass>]
type CoreEdge(edge: IBasicEdge, pool: CorePool, repo: IBasicRepository) =
    inherit CoreElement(edge, pool, repo)
    interface ICoreEdge with
        member this.Source 
            with get () =
                pool.Wrap edge.Source
            and set v =
                edge.Source <- (v :?> CoreElement).UnderlyingElement

        member this.Target
            with get () =
                pool.Wrap edge.Target
            and set v =
                edge.Target <- (v :?> CoreElement).UnderlyingElement
