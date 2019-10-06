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

/// Implementation of core repository as a wrapper around basic repository.
type CoreRepository(pool: CorePool, repo: BasicMetamodel.IBasicRepository) =

    member this.UnderlyingRepo = repo

    interface ICoreRepository with

        member this.Models =
            let modelMetatype = repo.Node Consts.metamodelModel :> BasicMetamodel.IBasicElement
            let models = repo.Nodes |> Seq.filter (fun n -> n.Metatype = modelMetatype)
            models |> Seq.map pool.WrapModel

        member this.Model name =
            (this :> ICoreRepository).Models
            |> Seq.filter (fun m -> m.Name = name)
            |> Helpers.exactlyOneModel name

        member this.CreateModelWithoutMetamodel name =
            let modelMetatype = repo.Node Consts.metamodelModel
            let model = repo.CreateNode name
            repo.CreateEdge model modelMetatype Consts.instanceOfEdge |> ignore
            pool.WrapModel model


