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

/// Implementation of wrapper factory.
type InfrastructureFactory(repo: ILanguageRepository) =
    interface IInfrastructureFactory with
        member this.CreateElement element pool =
            match element with
            | :? ILanguageNode as n -> InfrastructureNode(n, pool, repo) :> IInfrastructureElement
            | :? ILanguageGeneralization as g -> InfrastructureGeneralization(g, pool, repo) :> IInfrastructureElement
            | :? ILanguageInstanceOf as i -> InfrastructureInstanceOf(i, pool, repo) :> IInfrastructureElement
            | :? ILanguageAssociation as a -> InfrastructureAssociation(a, pool, repo) :> IInfrastructureElement
            | _ -> failwith "Unknown subtype"

        member this.CreateModel model pool =
            InfrastructureModel(model, pool, repo) :> IInfrastructureModel
