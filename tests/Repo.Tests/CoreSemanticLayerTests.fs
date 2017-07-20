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
    (fun () -> Repo.findModel repo "TestModel" |> ignore) |> should throw typeof<Repo.MultipleModelsWithGivenNameException>
