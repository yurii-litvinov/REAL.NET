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

module CoreSemanticLayerTests

open NUnit.Framework
open FsUnit

open Repo.Metametamodels
open Repo.DataLayer
open Repo.CoreSemanticLayer

let init () =
    let repo = DataRepo() :> IRepo
    let build (builder: IModelBuilder) =
        builder.Build repo

    CoreMetametamodelBuilder() |> build

    repo

[<Test>]
let ``Repo is able to find Core Metametamodel`` () =
    let repo = init()
    (Repo.coreMetamodel repo).Name |> should equal "CoreMetametamodel"

[<Test>]
let ``Repo is able to find any model`` () =
    let repo = init()
    let model1 = repo.CreateModel("TestModel1")
    let model2 = repo.CreateModel("TestModel2", model1)
    Repo.findModel repo "TestModel1" |> should sameAs model1
    Repo.findModel repo "TestModel2" |> should sameAs model2

[<Test>]
let ``Repo shall throw if no model found`` () =
    let repo = init()
    (fun () -> Repo.findModel repo "TestModel" |> ignore) |> should throw typeof<Repo.ModelNotFoundException>

[<Test>]
let ``Repo shall throw if searching two models with the same name`` () =
    let repo = init()
    let model1 = repo.CreateModel("TestModel")
    let model2 = repo.CreateModel("TestModel", model1)
    (fun () -> Repo.findModel repo "TestModel" |> ignore)
            |> should throw typeof<Repo.MultipleModelsException>

[<Test>]
let ``isInstanceOf shall work for long instantiation chains`` () =
    let repo = init()
    let model1 = repo.CreateModel("TestModel")
    let coreMetamodel = Repo.findModel repo "CoreMetametamodel"
    let node = Model.findNode coreMetamodel "Node"

    let element = model1.CreateNode("Element", node)
    let model2 = repo.CreateModel("TestModel2", model1)
    let instance = model2.CreateNode("Instance", element)

    Element.isInstanceOf element instance |> should be True
    Element.isInstanceOf node element |> should be True
    Element.isInstanceOf node instance |> should be True
    Element.isInstanceOf instance node |> should be False

[<Test>]
let ``isInstanceOf shall respect generalization`` () =
    let repo = init()
    let model1 = repo.CreateModel("TestModel")
    let coreMetamodel = Repo.findModel repo "CoreMetametamodel"
    let node = Model.findNode coreMetamodel "Node"
    let generalization = Model.findNode coreMetamodel "Generalization"

    let parent = model1.CreateNode("Parent", node)
    let descendant = model1.CreateNode("Descendant", node)
    model1.CreateGeneralization(generalization, descendant, parent) |> ignore
    let model2 = repo.CreateModel("TestModel2", model1)
    let descendantInstance = model2.CreateNode("descendantInstance", descendant)
    let parentInstance = model2.CreateNode("parentInstance", parent)

    Element.isInstanceOf descendant descendantInstance |> should be True
    Element.isInstanceOf parent descendantInstance |> should be True

    Element.isInstanceOf descendant parentInstance |> should be False
    Element.isInstanceOf parent parentInstance |> should be True

[<Test>]
let ``Setting attribute value in descendant shall not affect parent nor siblings`` () =
    let repo = init()
    let model1 = repo.CreateModel("TestModel")
    let coreMetamodel = Repo.findModel repo "CoreMetametamodel"
    let node = Model.findNode coreMetamodel "Node"
    let string = Model.findNode coreMetamodel "String"
    let association = Model.findNode coreMetamodel "Association"
    let generalization = Model.findNode coreMetamodel "Generalization"

    let parent = model1.CreateNode("Parent", node)
    let descendant1 = model1.CreateNode("Descendant1", node)
    let descendant2 = model1.CreateNode("Descendant2", node)
    model1.CreateGeneralization(generalization, descendant1, parent) |> ignore
    model1.CreateGeneralization(generalization, descendant2, parent) |> ignore

    Element.addAttribute parent "attribute" string association "attributeValueInParent"

    Element.attributeValue parent "attribute" |> should equal "attributeValueInParent"
    Element.attributeValue descendant1 "attribute" |> should equal "attributeValueInParent"
    Element.attributeValue descendant2 "attribute" |> should equal "attributeValueInParent"

    Element.setAttributeValue descendant1 "attribute" "attributeValueInDescendant"

    Element.attributeValue parent "attribute" |> should equal "attributeValueInParent"
    Element.attributeValue descendant1 "attribute" |> should equal "attributeValueInDescendant"
    Element.attributeValue descendant2 "attribute" |> should equal "attributeValueInParent"
