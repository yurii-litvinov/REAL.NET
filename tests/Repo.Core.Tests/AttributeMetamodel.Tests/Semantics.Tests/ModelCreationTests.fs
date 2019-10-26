(* Copyright 2019 REAL.NET group
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

namespace Repo.AttributeMetamodel.Semantics.Tests

open Repo
open Repo.AttributeMetamodel

open NUnit.Framework
open FsUnitTyped

/// Creation of two-level metamodel with Attribute Metamodel semantics.
/// Based on metamodels from J. de Lara and E. Guerra, "Deep Meta-Modelling with MetaDepth", 2010
[<TestFixture>]
type ModelCreationTests() =

    [<Test>]
    member this.``There shall be possible to create two-level hierarchy based on Attribute Metamodel`` () =
        let repo = AttributeMetamodelRepoFactory.Create ()
        let metamodel = repo.InstantiateAttributeMetamodel "Metamodel"

        let double = metamodel.CreateNode "double"

        let productType = metamodel.CreateNode "ProductType"
        let product = metamodel.CreateNode "Product"

        productType.AddAttribute "VAT" double
        product.AddAttribute "price" double

        let typeAssociation = metamodel.CreateAssociation product productType "type"


        let model = repo.InstantiateModel "Model" metamodel

        let num value = model.InstantiateNode (value.ToString ()) double Map.empty :> IAttributeElement
        
        let book = model.InstantiateNode "Book" productType (Map.empty.Add ("VAT", num 7))
        let cd = model.InstantiateNode "CD" productType (Map.empty.Add ("VAT", num 10.5))
        let mobyDick = model.InstantiateNode "mobyDick" product (Map.empty.Add ("price", num 10))
        let tosca = model.InstantiateNode "Tosca" product (Map.empty.Add ("price", num 16))

        let _ = model.InstantiateAssociation mobyDick book typeAssociation
        let _ = model.InstantiateAssociation tosca cd typeAssociation

        ((book.Slot "VAT").Value :?> IAttributeNode).Name |> shouldEqual "7"

        ()
