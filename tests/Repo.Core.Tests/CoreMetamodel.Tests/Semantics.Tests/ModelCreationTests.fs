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

namespace Repo.CoreMetamodel.Semantics.Tests

open Repo
open Repo.CoreMetamodel

open NUnit.Framework
open FsUnitTyped

/// Creation of two-level metamodel with Core Metamodel semantics.
/// Based on metamodels from J. de Lara and E. Guerra, "Deep Meta-Modelling with MetaDepth", 2010
[<TestFixture>]
type ModelCreationTests() =

    [<Test>]
    member this.``There shall be possible to create two-level hierarchy based on Core Metamodel`` () =
        let repo = CoreMetamodelRepoFactory.Create ()
        let metamodel = repo.InstantiateCoreMetamodel "Metamodel"

        let productType = metamodel.CreateNode "ProductType"
        let product = metamodel.CreateNode "Product"

        let vat = metamodel.CreateNode "VAT"
        let price = metamodel.CreateNode "price"

        let vatAssociation = metamodel.CreateAssociation productType vat "attribute"
        let priceAssociation = metamodel.CreateAssociation product price "attribute"

        let typeAssociation = metamodel.CreateAssociation product productType "type"


        let model = repo.InstantiateModel "Model" metamodel
        
        let book = model.InstantiateNode "Book" productType
        let cd = model.InstantiateNode "CD" productType
        let mobyDick = model.InstantiateNode "mobyDick" product
        let tosca = model.InstantiateNode "Tosca" product

        let _ = model.InstantiateAssociation mobyDick book typeAssociation
        let _ = model.InstantiateAssociation tosca cd typeAssociation

        let seven = model.CreateNode "7"
        let ten = model.CreateNode "10"
        let tenAndAHalf = model.CreateNode "10.5"
        let sixteen = model.CreateNode "16"

        let _ = model.InstantiateAssociation book seven vatAssociation
        let _ = model.InstantiateAssociation mobyDick ten priceAssociation
        let _ = model.InstantiateAssociation cd tenAndAHalf vatAssociation
        let _ = model.InstantiateAssociation tosca sixteen priceAssociation

        ()
