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

module FacadeModelTests

open NUnit.Framework
open FsUnit

open Repo
open Repo.FacadeLayer

let init () =
    let repo = new DataLayer.DataRepo()

    let build (builder: DataLayer.IModelBuilder) =
        builder.Build repo

    CoreModel.CoreModelBuilder() |> build
    Metametamodels.LanguageMetamodelBuilder() |> build
    Metametamodels.InfrastructureMetamodelBuilder() |> build

    let infrastructure = InfrastructureSemanticLayer.InfrastructureSemantic(repo)

    let attributeRepository = AttributeRepository()
    let elementRepository = ElementRepository(infrastructure, attributeRepository)
    let modelRepository = ModelRepository(infrastructure, elementRepository)

    let infrastructureMetamodel = infrastructure.Metamodel.Model

    let underlyingModel = (repo :> DataLayer.IDataRepository).CreateModel("Model", infrastructureMetamodel)
    let model = Model(infrastructure, underlyingModel, elementRepository, modelRepository)

    model :> IModel

[<Test>]
let ``Model shall allow to set a name`` () =
    let model = init ()
    model.Name |> should equal "Model"

    model.Name <- "Renamed"
    model.Name |> should equal "Renamed"

[<Test>]
let ``Model shall have metamodel`` () =
    let model = init ()
    let metamodel = model.Metamodel
    metamodel.Name |> should equal "InfrastructureMetamodel"

[<Test>]
let ``Model shall allow to create nodes`` () =
    let model = init ()
    let ``type`` = model.Metamodel.Nodes |> Seq.find (fun n -> n.Name = "Node")
    let node = model.CreateElement ``type``
    node.Metatype |> should equal Metatype.Node

[<Test>]
let ``Model shall allow to list its nodes`` () =
    let repo = RepoFactory.Create ()

    let model = repo.Model "RobotsTestModel"

    model.Nodes |> should not' (be Empty)
    model.Nodes |> Seq.filter (fun n -> n.Name = "aFinalNode") |> should not' (be Empty)
