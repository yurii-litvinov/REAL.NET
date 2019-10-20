(* Copyright 2019 REAL.NET Group
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

namespace Repo.InfrastructureMetamodel

open Repo
open Repo.InfrastructureMetamodel.Details
open Repo.InfrastructureMetamodel.Details.Elements

/// Factory that creates infrastructure metamodel repository.
[<AbstractClass; Sealed>]
type InfrastructureMetamodelRepoFactory =
    /// Method that returns repository with Core Metamodel.
    static member Create() = 
        let coreRepo = LanguageMetamodel.LanguageMetamodelRepoFactory.Create ()
        let factory = InfrastructureFactory(coreRepo)
        let pool = InfrastructurePool(factory)
        let repo = InfrastructureRepository(pool, coreRepo)
        InfrastructureMetamodelCreator.createIn repo
        repo :> IInfrastructureRepository

