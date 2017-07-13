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

module FacadeRepoTests

open NUnit.Framework
open FsUnit

open RepoExperimental

[<Test>]
let ``Repository shall contain at least core metamodel`` () =
    let repo = RepoFactory.CreateRepo ()

    repo.Models |> should not' (be Empty)

    // SHould throw if not found.
    repo.Models |> Seq.find (fun m -> m.Name = "CoreMetametamodel") |> ignore

[<Test>]
let ``Repository shall allow to add a model`` () =
    let repo = RepoFactory.CreateRepo ()

    repo.Models |> should not' (be Empty)
    let length = repo.Models |> Seq.length

    let coreMetamodel = repo.Models |> Seq.head
    repo.CreateModel("TestModel", coreMetamodel) |> ignore

    repo.Models |> Seq.length |> should equal (length + 1)

[<Test>]
let ``Repository shall allow to delete a model`` () =
    let repo = RepoFactory.CreateRepo ()

    repo.Models |> should not' (be Empty)
    let length = repo.Models |> Seq.length

    let someMetamodel = repo.Models |> Seq.head
    repo.DeleteModel someMetamodel

    repo.Models |> Seq.length |> should equal (length - 1)

[<Test>]
let ``Repository shall not allow to delete a model that is needed`` () =
    let repo = RepoFactory.CreateRepo ()

    repo.Models |> should not' (be Empty)

    let coreMetamodel = repo.Models |> Seq.head
    repo.CreateModel("TestModel", coreMetamodel) |> ignore

    (fun () -> repo.DeleteModel coreMetamodel) |> should throw typeof<DeletingUsedModel>
