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

open Repo
open System.IO
open Repo.Serializer.Serializer

let tempFile = Path.Combine(TestContext.CurrentContext.TestDirectory, "temp.rns")

[<SetUp>]
let setup () =
    if File.Exists tempFile then
        File.Delete tempFile

[<TearDown>]
let teardown () =
    //if File.Exists tempFile then
    //    File.Delete tempFile
    ()

[<Test>]
let ``Repository shall contain at least core metamodel`` () =
    let repo = RepoFactory.Create ()

    repo.Models |> should not' (be Empty)

    // Should throw if not found.
    repo.Models |> Seq.find (fun m -> m.Name = "CoreMetametamodel") |> ignore

[<Test>]
let ``Repository shall allow to add a model`` () =
    let repo = RepoFactory.Create ()

    repo.Models |> should not' (be Empty)
    let length = repo.Models |> Seq.length

    let coreMetamodel = repo.Models |> Seq.head
    repo.CreateModel("TestModel", coreMetamodel) |> ignore

    repo.Models |> Seq.length |> should equal (length + 1)

[<Test>]
let ``Repository shall allow to delete a model`` () =
    let repo = RepoFactory.Create ()

    repo.Models |> should not' (be Empty)
    let length = repo.Models |> Seq.length

    let someMetamodel = repo.Models |> Seq.head
    repo.DeleteModel someMetamodel

    repo.Models |> Seq.length |> should equal (length - 1)

[<Test>]
let ``Repository shall not allow to delete a model that is needed`` () =
    let repo = RepoFactory.Create ()

    repo.Models |> should not' (be Empty)

    let coreMetamodel = repo.Models |> Seq.head
    repo.CreateModel("TestModel", coreMetamodel) |> ignore

    (fun () -> repo.DeleteModel coreMetamodel) |> should throw typeof<DeletingUsedModel>

[<Test>]
let ``Repository shall be able to save into a file`` () =
    let repo = RepoFactory.Create ()
    repo.Save tempFile
    File.Exists tempFile |> should be True

[<Test>]
let ``Repository shall be able to load from file`` () =
    let repo = RepoFactory.Create ()
//    repo.Load (Path.Combine(TestContext.CurrentContext.TestDirectory, "Data/test.rns"))
    ()

[<Test>]
let ``After save/load repository shall contain an element`` () =
    let repo = RepoFactory.Create ()
    let model = repo.CreateModel("TestModel", "InfrastructureMetamodel")
    let node = (repo.Model "InfrastructureMetamodel").FindElement "Node"
    let newNode = model.CreateElement node
    newNode.Name <- "TestNode"

    repo.Save tempFile

    let repo = RepoFactory.Load tempFile

    let model = repo.Model "TestModel"
    let element = model.FindElement "TestNode"
    element.Name |> should equal "TestNode"

[<Test>]
let ``Graph with two nodes and one edge shall be serialized/deserialized properly`` () =
    let repo = RepoFactory.Create ()
    let model = repo.CreateModel("TestModel", "InfrastructureMetamodel")
    let newNode1 = model.CreateElement "Node"
    newNode1.Name <- "TestNode1"

    let newNode2 = model.CreateElement "Node"
    newNode2.Name <- "TestNode2"

    let association = (model.CreateElement "Association") :?> IEdge
    association.From <- newNode1
    association.To <- newNode2

    repo.Save tempFile

    let repo = RepoFactory.Load tempFile

    let model = repo.Model "TestModel"
    let node1 = model.FindElement "TestNode1"
    node1.Name |> should equal "TestNode1"

    let node2 = model.FindElement "TestNode2"
    node2.Name |> should equal "TestNode2"

    model.Edges |> Seq.length |> should equal 1

    let association = model.Edges |> Seq.head

    association.From |> should equal node1
    association.To |> should equal node2
