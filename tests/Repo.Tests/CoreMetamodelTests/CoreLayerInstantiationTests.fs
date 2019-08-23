(* Copyright 2019 Yurii Litvinov
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

module CoreLayerInstantiationTests

open NUnit.Framework
open FsUnit

open Repo.CoreMetamodel

let init () = TestUtils.init [CoreMetamodelCreator()]

[<Test>]
let ``Core Metamodel shall be able to be reinstantiated from itself`` () =
    let model = CoreSemanticsModelBuilder("Model")
    let (~+) name = model.AddNode(name)

    let node = +"Node"
    let element = +"Element"
    let edge = +"Edge"
    let generalization = +"Generalization"
    let association = +"Association"
    let stringNode = +"String"

    let (--|>) child parent = model.AddGeneralization child parent

    node --|> element
    edge --|> element
    generalization --|> edge
    association --|> edge

    let (--->) source (target, name) = model.AddAssociationByName source target name |> ignore

    element ---> (element, "ontologicalType")
    element ---> (element, "linguisticType")
    edge ---> (element, "source")
    edge ---> (element, "target")
    association ---> (stringNode, "targetName")

    let metaNode = model.Model.OntologicalMetamodel.Node "Node"

    model.Model.Nodes |> Seq.iter (fun e -> e.OntologicalType |> should equal metaNode)
