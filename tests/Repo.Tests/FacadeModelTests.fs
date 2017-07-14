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

open RepoExperimental
open RepoExperimental.FacadeLayer

let init () =
    let modelRepository = ModelRepository()

    let underlyingMetaModel = DataLayer.DataModel "Metamodel" :> DataLayer.IModel
    underlyingMetaModel.CreateNode "TypeNode" |> ignore

    let underlyingModel = DataLayer.DataModel("Model", underlyingMetaModel) :> DataLayer.IModel
    let model = Model(underlyingModel, modelRepository)
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
    metamodel.Name |> should equal "Metamodel"

[<Test>]
[<Ignore("Element instantiation is not implemented yet.")>]
let ``Model shall allow to create nodes`` () =
    let model = init ()
    let ``type`` = model.Metamodel.Nodes |> Seq.head
    let node = model.CreateElement ``type``
    node.Metatype |> should equal Metatype.Node

[<Test>]
let ``Model shall allow to list its nodes`` () =
    let repo = RepoFactory.CreateRepo ()

    let model = repo.Models |> Seq.find (fun m -> m.Name = "RobotsTestModel")

    model.Nodes |> Seq.length |> should equal 10
    model.Nodes |> Seq.filter (fun n -> n.Name = "anInitialNode") |> should not' (be Empty)