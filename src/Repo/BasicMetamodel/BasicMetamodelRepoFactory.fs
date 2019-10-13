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

namespace Repo.BasicMetamodel

open Repo.BasicMetamodel.Details

/// Factory that creates basic metamodel repository.
[<AbstractClass; Sealed>]
type BasicMetamodelRepoFactory =
    /// Method that returns repository with Basic Metamodel.
    static member Create() = 
        let repo = Elements.BasicRepository ()
        BasicMetamodelCreator.createIn repo
        repo :> IBasicRepository
