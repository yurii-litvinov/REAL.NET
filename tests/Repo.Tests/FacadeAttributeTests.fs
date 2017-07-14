﻿(* Copyright 2017 Yurii Litvinov
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
 
module FacadeAttributeTests

open NUnit.Framework
open FsUnit

open RepoExperimental
open RepoExperimental.FacadeLayer

let getAttributeType nodeName name =
    let repo = RepoFactory.CreateRepo ()
    let model = repo.Models |> Seq.find (fun m -> m.Name = "RobotsMetamodel")
    let attributeRepository = AttributeRepository ()

    let dataLayerModel = (model :?> Model).UnderlyingModel
    let dataLayerElement = dataLayerModel.Nodes |> Seq.find (fun n -> n.Name = nodeName) :> DataLayer.IElement

    attributeRepository.GetAttribute dataLayerModel dataLayerElement name

let getAttributeInstance nodeName name =
    let repo = RepoFactory.CreateRepo ()
    let model = repo.Models |> Seq.find (fun m -> m.Name = "RobotsTestModel")
    let metamodel = model.Metamodel
    let attributeRepository = AttributeRepository ()

    let dataLayerModel = (model :?> Model).UnderlyingModel
    let dataLayerMetamodel = (model.Metamodel :?> Model).UnderlyingModel
    
    let dataLayerClass = dataLayerMetamodel.Nodes |> Seq.find (fun n -> n.Name = nodeName) :> DataLayer.IElement
    let dataLayerElement = dataLayerModel.Nodes |> Seq.find (fun n -> n.Class = dataLayerClass) :> DataLayer.IElement

    attributeRepository.GetAttribute dataLayerModel dataLayerElement name

[<Test>]
let ``Attribute kind in metamodel for Motors Forward Ports is String`` () =
    let attribute = getAttributeType "MotorsForward" "ports"
    attribute.Kind |> should equal AttributeKind.String

[<Test>]
let ``Attribute kind in model for an instance of Motors Forward Ports is String`` () =
    let attribute = getAttributeInstance "MotorsForward" "ports"
    attribute.Kind |> should equal AttributeKind.String

[<Test>]
let ``Attribute value in metamodel for Motors Forward Ports is empty`` () =
    let attribute = getAttributeType "MotorsForward" "ports"
    attribute.StringValue |> should be Empty

[<Test>]
let ``Attribute value in model for an instance of Motors Forward Ports is 'M3, M4'`` () =
    let attribute = getAttributeInstance "MotorsForward" "ports"
    attribute.StringValue |> should equal "M3, M4"

[<Test>]
let ``Shall not allow to modify attribute value in metamodel`` () =
    let attribute = getAttributeType "MotorsForward" "ports"
    (fun () -> attribute.StringValue <- "Test") |> should throw typeof<System.Exception>

[<Test>]
let ``Attribute value in model can be changed`` () =
    let attribute = getAttributeInstance "MotorsForward" "ports"
    attribute.StringValue <- "M1, M2"
    attribute.StringValue |> should equal "M1, M2"