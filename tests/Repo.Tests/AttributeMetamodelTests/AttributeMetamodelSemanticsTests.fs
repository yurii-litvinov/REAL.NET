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

module AttributeMetamodelSemanticsTests

open NUnit.Framework
open FsUnit

open Repo.AttributeMetamodel

let init () = TestUtils.init [Repo.CoreMetamodel.CoreMetamodelCreator(); AttributeMetamodelCreator()]

[<Test>]
let ``Reintroduction of an attribute shall fail`` () =
    let model1creator = AttributeSemanticsModelBuilder("TestModel")

    let intNode = model1creator.AddNode "Int" []

    let parent = model1creator.AddNode "Parent" ["Attribute1"]
    let descendant = model1creator.AddNode "Descendant" []
    model1creator.AddAttributeWithType descendant "Attribute1" intNode

    (fun () -> model1creator.AddGeneralization descendant parent) |> should throw typeof<System.Exception>
