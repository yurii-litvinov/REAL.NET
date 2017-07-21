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
    let model = repo.CreateModel ("TestModel", CoreSemanticLayer.Repo.findModel repo "InfrastructureMetamodel")
    let elementHelper = InfrastructureSemanticLayer.ElementHelper repo
    let infrastructureMetamodel = elementHelper.InfrastructureMetamodel.InfrastructureMetamodel

    let node = CoreSemanticLayer.Model.findNode infrastructureMetamodel "Node"
    
    let element = Operations.instantiate repo model node

    let outgoingAssociations = CoreSemanticLayer.Element.outgoingAssociations element
    outgoingAssociations |> Seq.map (fun a -> (a.Target.Value :?> INode).Name) |> should contain "shape"
    let attributes = elementHelper.Attributes element
    attributes |> should not' (be Empty)
    elementHelper.AttributeValue element "shape" |> should equal "Pictures/Vertex.png"

[<Test>]
let ``Double instantiation shall result in correct instantiation chain`` () =
    let repo = init ()
    let metamodel = repo.CreateModel ("TestMetamodel", CoreSemanticLayer.Repo.findModel repo "InfrastructureMetamodel")
    let model = repo.CreateModel ("TestModel", metamodel)

    let elementHelper = InfrastructureSemanticLayer.ElementHelper repo
    let infrastructureMetamodel = elementHelper.InfrastructureMetamodel.InfrastructureMetamodel

    let node = CoreSemanticLayer.Model.findNode elementHelper.InfrastructureMetamodel.InfrastructureMetamodel "Node"
    
    let element = Operations.instantiate repo metamodel node

    let attribites = 
        elementHelper.Attributes element 
        |> Seq.map (fun attr -> CoreSemanticLayer.Element.attributeValue attr "stringValue")

    let elementInstance = Operations.instantiate repo model element

    elementInstance.Class.Class |> should equal node
