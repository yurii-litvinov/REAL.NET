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

open Repo.DataLayer
open Repo.Metametamodels
open Repo.CoreModel

[<Test>]
let ``Repo shall allow to create Type-Object-style model hierarchy`` () =
    let builder = CoreSemanticsModelCreator("Metamodel")

    let productType = builder.AddNode "ProductType" ["VAT"]
    let product = builder.AddNode "Product" ["price"]
    let typeAssociation = builder.AddAssociation product productType "type"

    let modelBuilder = builder.CreateInstanceModelBuilder "Model"

    let book = modelBuilder.InstantiateNode "Book" productType ["VAT", "7"]
    let mobyDick = modelBuilder.InstantiateNode "mobyDick" product ["price", "10"]
    modelBuilder.InstantiateEdge mobyDick book typeAssociation |> ignore

    let cd = modelBuilder.InstantiateNode "CD" productType ["VAT", "10.5"]
    let tosca = modelBuilder.InstantiateNode "Tosca" product ["price", "16"]
    modelBuilder.InstantiateEdge tosca cd typeAssociation |> ignore

    Element.attributeValue cd "VAT" |> should equal "10.5"
    Element.hasAttribute tosca "VAT" |> should be False
    Element.attributeValue tosca "price" |> should equal "16"

    Element.attributeValue tosca "type" |> should equal "CD"

    // VAT has a type of String (as anything else in models derived from Core metamodel
    Element.attributeValue productType "VAT" |> should equal "String"

    // Assigning value to an attribute that was created without a value (so it is an attribute, not a field) 
    // should be impossible.
    (fun () -> Element.setAttributeValue productType "VAT" "Test") 
            |> should throw (typeof<Repo.InvalidSemanticOperationException>)

    // Some model consistency checks:
    // 1. Strictness of metalayers --- each element in model is an instance of an element in metamodel.
    modelBuilder.Model.Elements |> Seq.forall (fun e -> e.Class.Model = builder.Model) |> should be True

    // 2. There are no associations crossing metalayers.
    modelBuilder.Model.Edges |> Seq.forall (fun e -> e.Source.Value.Model = e.Target.Value.Model) |> should be True
    ()


[<Test>]
let ``Repo shall allow to create deep model hierarchy`` () =
    let model2Builder = CoreSemanticsModelCreator("Model@2")

    let productType = model2Builder.AddNode "ProductType" ["VAT"; "price"]

    let model1Builder = model2Builder.CreateInstanceModelBuilder "Model@1"

    let book = model1Builder.InstantiateNode "Book" productType ["VAT", "7"]
    let cd = model1Builder.InstantiateNode "CD" productType ["VAT", "10.5"]

    let model0Builder = model1Builder.CreateInstanceModelBuilder "Model@0"

    let mobyDick = model0Builder.InstantiateNode "mobyDick" book ["price", "10"]
    let tosca = model0Builder.InstantiateNode "Tosca" cd ["price", "16"]

    Element.attributeValue cd "VAT" |> should equal "10.5"
    Element.hasAttribute tosca "VAT" |> should be False
    Element.attributeValue tosca "price" |> should equal "16"

    Element.attributeValue tosca "type" |> should equal "CD"

    // VAT has a type of String (as anything else in models derived from Core metamodel
    Element.attributeValue productType "VAT" |> should equal "String"

    // Assigning value to an attribute that was created without a value (so it is an attribute, not a field) 
    // should be impossible.
    (fun () -> Element.setAttributeValue productType "VAT" "Test") 
            |> should throw (typeof<Repo.InvalidSemanticOperationException>)

    ()
