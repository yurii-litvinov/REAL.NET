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

module FacadeNodeTest

open NUnit.Framework
open FsUnit

open Repo
open Repo.FacadeLayer

[<Test>]
let ``Node in a model shall have metatype`` () =
    let repo = RepoFactory.Create ()
    let underlyingRepo = (repo :?> Repo).UnderlyingRepo

    let model = repo.Models |> Seq.find (fun m -> m.Name = "RobotsTestModel")
    let dataLayerModel = (model :?> Model).UnderlyingModel

    let metamodel = repo.Models |> Seq.find (fun m -> m.Name = "RobotsMetamodel")
    let dataLayerMetamodel = (metamodel :?> Model).UnderlyingModel

    let dataLayerClass = dataLayerMetamodel.Nodes |> Seq.find (fun n -> n.Name = "MotorsForward") :> DataLayer.IElement
    let dataLayerElement = dataLayerModel.Nodes |> Seq.find (fun n -> n.Class = dataLayerClass)

    let infrastructure = InfrastructureSemanticLayer.InfrastructureSemantic(underlyingRepo)

    let attributeRepository = AttributeRepository()
    let elementRepository = ElementRepository(infrastructure, attributeRepository) :> IElementRepository

    let motorsForwardInstanceNode = elementRepository.GetElement dataLayerElement

    motorsForwardInstanceNode.Metatype |> should equal Metatype.Node

[<Test>]
let ``Timer block shall have a picture`` () =
    let repo = RepoFactory.Create ()
    let underlyingRepo = (repo :?> Repo).UnderlyingRepo

    let model = repo.Models |> Seq.find (fun m -> m.Name = "RobotsTestModel")
    let dataLayerModel = (model :?> Model).UnderlyingModel

    let metamodel = repo.Models |> Seq.find (fun m -> m.Name = "RobotsMetamodel")
    let dataLayerMetamodel = (metamodel :?> Model).UnderlyingModel

    let dataLayerClass = dataLayerMetamodel.Nodes |> Seq.find (fun n -> n.Name = "Timer") :> DataLayer.IElement
    let dataLayerElement = dataLayerModel.Nodes |> Seq.find (fun n -> n.Class = dataLayerClass)

    let infrastructure = InfrastructureSemanticLayer.InfrastructureSemantic(underlyingRepo)

    let attributeRepository = AttributeRepository()
    let elementRepository = ElementRepository(infrastructure, attributeRepository) :> IElementRepository

    let timer = elementRepository.GetElement dataLayerElement

    timer.Class.Shape |> should equal "View/Pictures/timerBlock.png"
