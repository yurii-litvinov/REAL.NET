(* Copyright 2019 Yurii Litvinov
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

namespace Repo.AttributeMetamodel

open Repo
open Repo.AttributeMetamodel.Details
open Repo.AttributeMetamodel.Details.Elements

/// Factory that creates attribute metamodel repository.
[<AbstractClass; Sealed>]
type AttributeMetamodelRepoFactory =
    /// Method that returns repository with Attribute Metamodel.
    static member Create() = 
        let coreRepo = CoreMetamodel.CoreMetamodelRepoFactory.Create ()
        let factory = AttributeFactory(coreRepo)
        let pool = AttributePool(factory)
        let repo = AttributeRepository(pool, coreRepo)
        AttributeMetamodelCreator.createIn repo
        repo :> IAttributeRepository

