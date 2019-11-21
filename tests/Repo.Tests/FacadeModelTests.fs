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
open System.IO

let tempFile = Path.Combine(TestContext.CurrentContext.TestDirectory, "temp.rns")

[<SetUp>]
let setup () =
    if File.Exists tempFile then
        File.Delete tempFile

let init () =
    let repo = new DataLayer.DataRepo()

    let build (builder: Metametamodels.IModelBuilder) =
        builder.Build repo

    Metametamodels.CoreMetametamodelBuilder() |> build
    Metametamodels.LanguageMetamodelBuilder() |> build
    Metametamodels.InfrastructureMetamodelBuilder() |> build

    let infrastructure = InfrastructureSemanticLayer.InfrastructureSemantic(repo)

    let attributeRepository = AttributeRepository()
    let elementRepository = ElementRepository(infrastructure, attributeRepository)
    let modelRepository = ModelRepository(infrastructure, elementRepository)

    let infrastructureMetamodel = infrastructure.Metamodel.Model

    let underlyingModel = (repo :> DataLayer.IRepo).CreateModel("Model", infrastructureMetamodel)
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
let ``Model shall allow to delete elements``() =
    let model = init()
    let ``type`` = model.Metamodel.Nodes |> Seq.find (fun n -> n.Name = "Node")
    let node = model.CreateElement ``type``
    node.AddAttribute ("Attribute1", AttributeKind.String, "default") |> ignore
    let unwrapped = (node :?> Element).UnderlyingElement
    let attribute = node.Attributes |> Seq.find (fun u -> u.Name = "Attribute1")
    let unwrappedAttribute = (attribute :?> Attribute).UnderlyingNode
    model.DeleteElement node
    model.Nodes |> should not' (contain node) 
    unwrapped.IsMarkedDeleted |> should be True
    unwrappedAttribute.IsMarkedDeleted |> should be True

[<Test>]
let ``Model shall allow to restore elements``() =
    let model = init()
    let ``type`` = model.Metamodel.Nodes |> Seq.find (fun n -> n.Name = "Node")
    let node = model.CreateElement ``type``
    node.AddAttribute ("Attribute1", AttributeKind.String, "default") |> ignore
    node.AddAttribute ("Attribute2", AttributeKind.Int, "0") |> ignore
    let unwrapped = (node :?> Element).UnderlyingElement
    let attributes = node.Attributes |> Seq.filter (fun a -> a.Name = "Attribute1" || a.Name = "Attribute2")
                     |> Seq.map (fun a -> (a :?> Attribute).UnderlyingNode)
    model.DeleteElement node
    model.Nodes |> should not' (contain node)
    model.RestoreElement node
    model.Nodes |> should contain node
    unwrapped.IsMarkedDeleted |> should be False
    attributes |> Seq.map (fun a -> a |> should be False) |> ignore

[<Test>]
let ``Model shall allow to list its nodes`` () =
    let repo = RepoFactory.Create ()

    let model = repo.Models |> Seq.find (fun m -> m.Name = "RobotsTestModel")

    let underlyingModel = (model :?> Model).UnderlyingModel
    let initialNode = CoreSemanticLayer.Model.findNode underlyingModel "aFinalNode"

    model.Nodes |> should not' (be Empty)
    model.Nodes |> Seq.filter (fun n -> n.Name = "aFinalNode") |> should not' (be Empty)

[<Test>]
let ``Model IsVisible attribute can be stored and changed`` () =
    let repo = RepoFactory.Create ()
    
    let model = repo.Models |> Seq.find (fun m -> m.Name = "RobotsTestModel")
    
    model.IsVisible |> should be True

    model.IsVisible <- false

    model.IsVisible |> should be False

    model.IsVisible <- true

    model.IsVisible |> should be True

[<Test>]
let ``Model IsVisible attribute shall be serialized/deserialized properly`` () =
    let repo = RepoFactory.Create ()
    
    let model = repo.Models |> Seq.find (fun m -> m.Name = "RobotsTestModel")
    
    model.IsVisible |> should be True

    model.IsVisible <- false

    model.IsVisible |> should be False

    repo.Save tempFile

    let repo = RepoFactory.Load tempFile

    let model = repo.Models |> Seq.find (fun m -> m.Name = "RobotsTestModel")

    model.IsVisible |> should be False
