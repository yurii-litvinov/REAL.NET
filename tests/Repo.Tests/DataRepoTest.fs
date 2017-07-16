(* Copyright 2017 Yurii Litvinov
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

module DataRepoTest

open NUnit.Framework
open FsUnit

open Repo.DataLayer

[<Test>]
let ``Repo shall be able to create a model`` () =
    let repo = DataRepo() :> IRepo
    let model = repo.CreateModel "model"

    repo.Models |> should contain model

[<Test>]
let ``Repo shall be able to delete a model`` () =
    let repo = DataRepo() :> IRepo
    let model = repo.CreateModel "model"

    repo.Models |> should contain model

    repo.DeleteModel model

    repo.Models |> should not' (contain model)

[<Test>]
let ``Repo shall throw if deleting model in use`` () =
    let repo = DataRepo() :> IRepo
    let metamodel = repo.CreateModel "metamodel"
    let model = repo.CreateModel("model", metamodel)

    repo.Models |> should contain model

    (fun () -> repo.DeleteModel metamodel) |> should throw typeof<Repo.DeletingUsedModel>
