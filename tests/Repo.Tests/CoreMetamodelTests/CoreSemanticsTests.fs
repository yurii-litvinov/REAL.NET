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

module CoreSemanticsTests

open NUnit.Framework
open FsUnit

open Repo.CoreMetamodel

let init () = TestUtils.init [CoreMetamodelCreator()]

[<Test>]
let ``Repo is able to find Core Metamodel`` () =
    let repo = init()
    (repo.Model "CoreMetamodel").Name |> should equal "CoreMetamodel"

[<Test>]
let ``Repo is able to find any model`` () =
    let repo = init()
    let model1 = repo.CreateModel("TestModel1")
    let model2 = repo.CreateModel("TestModel2", model1, model1)
    repo.Model "TestModel1" |> should sameAs model1
    repo.Model "TestModel2" |> should sameAs model2

[<Test>]
let ``Repo shall throw if no model found`` () =
    let repo = init()
    (fun () -> repo.Model "TestModel" |> ignore) |> should throw typeof<Repo.ModelNotFoundException>

[<Test>]
let ``Repo shall throw if searching two models with the same name`` () =
    let repo = init()
    let model1 = repo.CreateModel("TestModel")
    let _ = repo.CreateModel("TestModel", model1, model1)
    (fun () -> repo.Model "TestModel" |> ignore)
            |> should throw typeof<Repo.MultipleModelsException>

[<Test>]
let ``IsInstanceOf shall work for long instantiation chains`` () =
    let repo = init()
    let model1 = repo.CreateModel("TestModel")
    let coreMetamodel = repo.Model "CoreMetamodel"
    let node = coreMetamodel.Node "Node"

    let element = model1.CreateNode("Element", node, node)
    let model2 = repo.CreateModel("TestModel2", model1, model1)
    let instance = model2.CreateNode("Instance", element, element)

    ElementSemantics.IsOntologicalInstanceOf element instance |> should be True
    ElementSemantics.IsOntologicalInstanceOf node element |> should be True
    ElementSemantics.IsOntologicalInstanceOf node instance |> should be True
    ElementSemantics.IsOntologicalInstanceOf instance node |> should be False

[<Test>]
let ``IsInstanceOf shall respect generalization`` () =
    let repo = init()
    let model1 = repo.CreateModel("TestModel")
    let coreMetamodel = repo.Model "CoreMetamodel"
    let node = coreMetamodel.Node "Node"
    let generalization = coreMetamodel.Node "Generalization"

    let parent = model1.CreateNode("Parent", node, node)
    let descendant = model1.CreateNode("Descendant", node, node)
    model1.CreateGeneralization(generalization, generalization, descendant, parent) |> ignore
    let model2 = repo.CreateModel("TestModel2", model1, model1)
    let descendantInstance = model2.CreateNode("descendantInstance", descendant, descendant)
    let parentInstance = model2.CreateNode("parentInstance", parent, parent)

    ElementSemantics.IsOntologicalInstanceOf descendant descendantInstance |> should be True
    ElementSemantics.IsOntologicalInstanceOf parent descendantInstance |> should be True

    ElementSemantics.IsOntologicalInstanceOf descendant parentInstance |> should be False
    ElementSemantics.IsOntologicalInstanceOf parent parentInstance |> should be True
