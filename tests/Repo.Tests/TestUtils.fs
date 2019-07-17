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

module TestUtils

open Repo.DataLayer

/// Initializes a repository with given list of model builders.
let init (models: IModelBuilder list) =
    let repo = DataRepo() :> IDataRepository

    let build (builder: IModelBuilder) =
        builder.Build repo

    List.iter build models
    repo
