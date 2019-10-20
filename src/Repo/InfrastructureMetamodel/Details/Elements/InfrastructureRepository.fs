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

namespace Repo.InfrastructureMetamodel.Details.Elements

open Repo
open Repo.InfrastructureMetamodel
open Repo.LanguageMetamodel

/// Implementation of core repository as a wrapper around basic repository.
type InfrastructureRepository(pool: InfrastructurePool, repo: ILanguageRepository) =

    // Recalculated because repo is created empty and then filled with models.
    let infrastructureMetamodel () = repo.Model Consts.infrastructureMetamodel

    let wrap model = pool.WrapModel model
    let unwrap (model: IInfrastructureModel) = (model :?>InfrastructureModel).UnderlyingModel

    /// Returns underlying BasicRepository object.
    member this.UnderlyingRepo = repo

    interface IInfrastructureRepository with

        member this.Models = 
            repo.Models |> Seq.map wrap

        member this.Model name = 
            repo.Model name |> wrap

        member this.InstantiateAttributeMetamodel name =
            (this :> IInfrastructureRepository).InstantiateModel name (pool.WrapModel (infrastructureMetamodel ()))

        member this.InstantiateModel name metamodel = 
            repo.InstantiateModel name (unwrap metamodel) |> wrap

        member this.DeleteModel model =
            repo.DeleteModel (unwrap model)
            pool.UnregisterModel (unwrap model)

        member this.Clear () =
            pool.Clear ()
            repo.Clear ()
            ()
