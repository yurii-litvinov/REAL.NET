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
open Repo.CoreMetamodel

// Strictness of metalayers --- each element in model is an instance of an element in metamodel.
let checkMetalayerStrictness (creator: CoreSemanticsModelCreator) =
    creator.Model.Elements |> Seq.forall (fun e -> e.Class.Model = creator.Model.Metamodel) |> should be True

// Associations can not cross metalayers.
let checkAssociationsCantCrossMetalayers (creator: CoreSemanticsModelCreator) =
    creator.Model.Edges |> Seq.forall (fun e -> e.Source.Value.Model = e.Target.Value.Model) |> should be True 

[<Test>]
let ``Repo shall allow to create Type-Object-style model hierarchy`` () =
    let metamodelCreator = CoreSemanticsModelCreator("Metamodel")

    let productType = metamodelCreator.AddNode "ProductType" ["VAT"]
    let product = metamodelCreator.AddNode "Product" ["price"]
    let typeAssociation = metamodelCreator.AddAssociation product productType "type"

    let modelCreator = metamodelCreator.CreateInstanceModelBuilder "Model"

    let book = modelCreator.InstantiateNode "Book" productType ["VAT", "7"]
    let mobyDick = modelCreator.InstantiateNode "mobyDick" product ["price", "10"]
    modelCreator.InstantiateEdge mobyDick book typeAssociation |> ignore

    let cd = modelCreator.InstantiateNode "CD" productType ["VAT", "10.5"]
    let tosca = modelCreator.InstantiateNode "Tosca" product ["price", "16"]
    modelCreator.InstantiateEdge tosca cd typeAssociation |> ignore

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
    // 1. Strictness of metalayers
    checkMetalayerStrictness modelCreator
    
    // 2. There are no associations crossing metalayers.
    checkAssociationsCantCrossMetalayers modelCreator
    ()


[<Test>]
let ``Repo shall allow to create deep model hierarchy`` () =
    let model2Creator = CoreSemanticsModelCreator("Model@2")

    let productType = model2Creator.AddNode "ProductType" ["VAT"; "price"]

    let model1Creator = model2Creator.CreateInstanceModelBuilder "Model@1"

    let book = model1Creator.InstantiateNode "Book" productType ["VAT", "7"]
    // Here we need to reintroduce attribute because core semantics treats attributes as associations.
    model1Creator.AddAttribute book "price"

    let cd = model1Creator.InstantiateNode "CD" productType ["VAT", "10.5"]
    model1Creator.AddAttribute cd "price"

    let model0Creator = model1Creator.CreateInstanceModelBuilder "Model@0"

    let mobyDick = model0Creator.InstantiateNode "mobyDick" book ["price", "10"]
    let tosca = model0Creator.InstantiateNode "Tosca" cd ["price", "16"]

    Element.attributeValue cd "VAT" |> should equal "10.5"
    Element.hasAttribute tosca "VAT" |> should be False
    Element.attributeValue tosca "price" |> should equal "16"

    // VAT has a type of String (as anything else in models derived from Core metamodel
    Element.attributeValue productType "VAT" |> should equal "String"

    // Assigning value to an attribute that was created without a value (so it is an attribute, not a field) 
    // should be impossible.
    (fun () -> Element.setAttributeValue productType "VAT" "Test") 
            |> should throw (typeof<Repo.InvalidSemanticOperationException>)

    // Some model consistency checks:
    // 1. Strictness of metalayers --- each element in model is an instance of an element in metamodel.
    checkMetalayerStrictness model0Creator
    checkMetalayerStrictness model2Creator

    // Model@1 reintroduces attributes using Core model as their type, so it is a linguistic extension and breaks 
    // level hierarchy.

    // 2. There are no associations crossing metalayers.
    checkAssociationsCantCrossMetalayers model0Creator
    checkAssociationsCantCrossMetalayers model1Creator
    checkAssociationsCantCrossMetalayers model2Creator

    ()
