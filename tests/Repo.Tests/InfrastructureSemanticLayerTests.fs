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

module InfrastructureSemanticLayerTests

open NUnit.Framework
open FsUnit

open Repo
open Repo.Metametamodels
open Repo.DataLayer
open Repo.InfrastructureSemanticLayer

let init () =
    let repo = DataRepo() :> IRepo
    let build (builder: IModelBuilder) =
        builder.Build repo

    CoreMetametamodelBuilder() |> build
    LanguageMetamodelBuilder() |> build
    InfrastructureMetamodelBuilder() |> build

    repo

[<Test>]
let ``Instantiation shall preserve linguistic attributes`` () =
    let repo = init ()
    let infrastructure = InfrastructureSemantic(repo)
    let model = repo.CreateModel ("TestModel", infrastructure.Metamodel.Model)

    let node = infrastructure.Metamodel.Node

    let element = infrastructure.Instantiate model node

    let outgoingAssociations = CoreSemanticLayer.Element.outgoingAssociations element
    outgoingAssociations |> Seq.map (fun a -> (a.Target.Value :?> INode).Name) |> should contain "shape"
    let attributes = infrastructure.Element.Attributes element
    attributes |> should not' (be Empty)
    infrastructure.Element.AttributeValue element "shape" |> should equal "Pictures/Vertex.png"

[<Test>]
let ``Double instantiation shall result in correct instantiation chain`` () =
    let repo = init ()
    let infrastructure = InfrastructureSemantic(repo)
    let metamodel = repo.CreateModel ("TestMetamodel", infrastructure.Metamodel.Model)
    let model = repo.CreateModel ("TestModel", metamodel)
    let node = infrastructure.Metamodel.Node

    let element = infrastructure.Instantiate metamodel node

    let attribites =
        infrastructure.Element.Attributes element
        |> Seq.map (fun attr -> CoreSemanticLayer.Element.attributeValue attr "stringValue")

    let elementInstance = infrastructure.Instantiate model element

    elementInstance.Class.Class |> should equal node

[<Test>]
let ``Setting attribute value in descendant shall not affect parent nor siblings`` () =
    let repo = init ()
    let infrastructure = InfrastructureSemantic(repo)
    let model = repo.CreateModel ("TestModel", infrastructure.Metamodel.Model)

    let parent = model.CreateNode("Parent", infrastructure.Metamodel.Node)
    let descendant1 = model.CreateNode("Descendant1", infrastructure.Metamodel.Node)
    let descendant2 = model.CreateNode("Descendant2", infrastructure.Metamodel.Node)
    model.CreateGeneralization(infrastructure.Metamodel.Generalization, descendant1, parent) |> ignore
    model.CreateGeneralization(infrastructure.Metamodel.Generalization, descendant2, parent) |> ignore

    infrastructure.Element.AddAttribute parent "attribute" "AttributeKind.String" |> ignore
    infrastructure.Element.SetAttributeValue parent "attribute" "attributeValueInParent"

    infrastructure.Element.AttributeValue parent "attribute" |> should equal "attributeValueInParent"
    infrastructure.Element.AttributeValue descendant1 "attribute" |> should equal "attributeValueInParent"
    infrastructure.Element.AttributeValue descendant2 "attribute" |> should equal "attributeValueInParent"

    infrastructure.Element.SetAttributeValue descendant1 "attribute" "attributeValueInDescendant"

    infrastructure.Element.AttributeValue parent "attribute" |> should equal "attributeValueInParent"
    infrastructure.Element.AttributeValue descendant1 "attribute" |> should equal "attributeValueInDescendant"
    infrastructure.Element.AttributeValue descendant2 "attribute" |> should equal "attributeValueInParent"
