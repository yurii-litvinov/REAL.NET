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

module AttributeMetamodelTests

open FsUnit
open NUnit.Framework

open Repo.CoreMetamodel
open Repo.AttributeMetamodel

let init () = TestUtils.init [CoreMetamodelCreator(); AttributeMetamodelCreator()]

[<Test>]
let ``Builder shall be able to create model in repo`` () =
    let repo = init()

    Seq.length repo.Models |> should equal 2

    (Seq.head repo.Models).Name |> should equal "AttributeMetamodel"
    (repo.Models |> Seq.skip 1 |> Seq.head).Name |> should equal "CoreMetamodel"

[<Test>]
let ``Attribute metamodel shall have attributes`` () =
    let repo = init ()

    Seq.length repo.Models |> should equal 2

    let attributeModel = repo.Model "AttributeMetamodel"

    attributeModel.HasNode "Attribute" |> should equal true

    let attributeNode = attributeModel.Node "Attribute"
    let element = attributeModel.Node "Element"
    let attributeAssociation = ElementSemantics.OutgoingAssociation element "attributes"
    attributeAssociation.Target.Value |> should equal <| attributeNode

    attributeNode.OutgoingEdges |> Seq.length |> should equal 1

    ElementSemantics.HasOutgoingAssociation attributeNode "type" |> should equal true
