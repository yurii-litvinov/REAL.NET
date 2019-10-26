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

namespace Repo.LanguageMetamodel.Details.Elements

open Repo
open Repo.LanguageMetamodel
open Repo.AttributeMetamodel

/// Implementation of core repository as a wrapper around basic repository.
type LanguageRepository(pool: LanguagePool, repo: IAttributeRepository) =

    // Recalculated because repo is created empty and then filled with models.
    let attributeMetamodel () = repo.Model Consts.attributeMetamodel

    let wrap model = pool.WrapModel model
    let unwrap (model: ILanguageModel) = (model :?> LanguageModel).UnderlyingModel

    /// Returns underlying BasicRepository object.
    member this.UnderlyingRepo = repo

    interface ILanguageRepository with

        member this.Models = 
            repo.Models |> Seq.map wrap

        member this.Model name = 
            repo.Model name |> wrap

        member this.InstantiateLanguageMetamodel name =
            (this :> ILanguageRepository).InstantiateModel name (pool.WrapModel (attributeMetamodel ()))

        member this.InstantiateModel name metamodel = 
            repo.InstantiateModel name (unwrap metamodel) |> wrap

        member this.DeleteModel model =
            repo.DeleteModel (unwrap model)
            pool.UnregisterModel (unwrap model)

        member this.Clear () =
            pool.Clear ()
            repo.Clear ()
            ()
