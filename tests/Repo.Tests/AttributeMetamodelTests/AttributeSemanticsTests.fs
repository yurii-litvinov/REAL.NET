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

module AttributeSemanticLayerTests

open NUnit.Framework
open FsUnit

open Repo.AttributeMetamodel

let init () = TestUtils.init [Repo.CoreMetamodel.CoreMetamodelBuilder(); AttributeMetamodelBuilder()]

[<Test>]
let ``Setting attribute value in descendant shall not affect parent nor siblings`` () =
    let model1creator = AttributeSemanticsModelCreator("TestModel")

    let parent = model1creator.AddNode "Parent" []
    let descendant1 = model1creator.AddNode "Descendant1" []
    let descendant2 = model1creator.AddNode "Descendant2" []
    model1creator.AddGeneralization descendant1 parent
    model1creator.AddGeneralization descendant2 parent

    let elementHelper = Element(model1creator.Repo)

    model1creator.AddAttributeWithValue parent "attribute" "attributeValueInParent"

    elementHelper.AttributeValue parent "attribute" |> should equal "attributeValueInParent"
    elementHelper.AttributeValue descendant1 "attribute" |> should equal "attributeValueInParent"
    elementHelper.AttributeValue descendant2 "attribute" |> should equal "attributeValueInParent"

    elementHelper.SetAttributeValue descendant1 "attribute" "attributeValueInDescendant"

    elementHelper.AttributeValue parent "attribute" |> should equal "attributeValueInParent"
    elementHelper.AttributeValue descendant1 "attribute" |> should equal "attributeValueInDescendant"
    elementHelper.AttributeValue descendant2 "attribute" |> should equal "attributeValueInParent"

[<Test>]
let ``Reintroduction of an attribute shall fail`` () =
    let model1creator = AttributeSemanticsModelCreator("TestModel")

    let parent = model1creator.AddNode "Parent" ["Attribute1"]
    let descendant = model1creator.AddNode "Descendant" ["Attribute1"]
    (fun () -> model1creator.AddGeneralization descendant parent) |> shouldFail
