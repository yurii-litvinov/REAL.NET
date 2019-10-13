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

namespace Repo.CoreMetamodel.Elements.Tests

open Repo
open Repo.CoreMetamodel
open Repo.CoreMetamodel.Details.Elements

open NUnit.Framework
open FsUnitTyped

[<TestFixture>]
type CoreElementTests() =

    let mutable repo = BasicMetamodel.BasicMetamodelRepoFactory.Create ()
    let mutable factory = CoreFactory(repo)
    let mutable pool = CorePool(factory)

    let (~+) name = 
        let unwrappedNode = repo.CreateNode name
        pool.Wrap (unwrappedNode)

    let (--->) (node1: ICoreElement) (node2: ICoreElement) =
        let unwrappedNode1 = (node1 :?> CoreElement).UnderlyingElement
        let unwrappedNode2 = (node2 :?> CoreElement).UnderlyingElement
        let unwrappedEdge = repo.CreateEdge unwrappedNode1 unwrappedNode2 "testEdge"
        pool.Wrap unwrappedEdge :?> ICoreAssociation

    [<Test>]
    member this.OutgoingEdgesTest () =
        let node1 = +"Node1"
        let node2 = +"Node2"
        let edge1 = node1 ---> node2
        let edge2 = node1 ---> node1

        node1.OutgoingEdges |> shouldHaveLength 2
        node1.OutgoingEdges |> shouldContain (edge1 :> ICoreEdge)
        node1.OutgoingEdges |> shouldContain (edge2 :> ICoreEdge)

        node2.OutgoingEdges |> shouldBeEmpty

    [<Test>]
    member this.OutgoingAssociationsTest () =
        let node1 = +"Node1"
        let node2 = +"Node2"
        let edge1 = node1 ---> node2
        let edge2 = node1 ---> node1

        node1.OutgoingAssociations |> shouldHaveLength 2
        node1.OutgoingAssociations |> shouldContain edge1
        node1.OutgoingAssociations |> shouldContain edge2

        node2.OutgoingAssociations |> shouldBeEmpty

    [<Test>]
    member this.OutgoingAssociationTest () =
        let node1 = +"Node1"
        let node2 = +"Node2"
        let edge1 = node1 ---> node2

        node1.OutgoingAssociation "testEdge" |> shouldEqual edge1
        (fun () -> node1.OutgoingAssociation "nonExistingEdge" |> ignore) |> shouldFail<ElementNotFoundException>

        let edge2 = node1 ---> node1

        (fun () -> node1.OutgoingAssociation "testEdge" |> ignore) |> shouldFail<MultipleElementsException>

    [<Test>]
    member this.IncomingEdgesTest () =
        let node1 = +"Node1"
        let node2 = +"Node2"
        let edge1 = node1 ---> node2
        let edge2 = node1 ---> node1

        node1.IncomingEdges |> shouldHaveLength 1
        node1.IncomingEdges |> shouldContain (edge2 :> ICoreEdge)

        node2.IncomingEdges |> shouldHaveLength 1
        node2.IncomingEdges |> shouldContain (edge1 :> ICoreEdge)

    [<Test>]
    member this.IncomingAssociationsTest () =
        let node1 = +"Node1"
        let node2 = +"Node2"
        let edge1 = node1 ---> node2
        let edge2 = node1 ---> node1

        node1.IncomingAssociations |> shouldHaveLength 1
        node1.IncomingAssociations |> shouldContain edge2

        node2.IncomingAssociations |> shouldHaveLength 1
        node2.IncomingAssociations |> shouldContain edge1

    [<Test>]
    member this.IncomingAssociationTest () =
        let node1 = +"Node1"
        let node2 = +"Node2"
        let edge1 = node1 ---> node2

        node2.IncomingAssociation "testEdge" |> shouldEqual edge1
        (fun () -> node2.IncomingAssociation "nonExistingEdge" |> ignore) |> shouldFail<ElementNotFoundException>

        let edge2 = node2 ---> node2

        (fun () -> node2.IncomingAssociation "testEdge" |> ignore) |> shouldFail<MultipleElementsException>

    [<Test>]
    member this.InstanceOfTest () =
        let coreRepo = CoreMetamodelRepoFactory.Create ()
        let metamodel = coreRepo.InstantiateCoreMetamodel "TestMetamodel"
        let model = coreRepo.InstantiateModel "Model" metamodel

        let node1 = metamodel.CreateNode "Node1"
        let node2 = model.InstantiateNode "Node2" node1

        node2.IsInstanceOf node1 |> shouldEqual true
        node2.Metatype |> shouldEqual (node1 :> ICoreElement)

        let coreNode = coreRepo.CoreMetamodel.Node Consts.node

        node2.IsInstanceOf coreNode |> shouldEqual true
        node1.IsInstanceOf coreNode |> shouldEqual true

        node1.IsInstanceOf node2 |> shouldEqual false

    [<Test>]
    member this.InstanceOfRespectsGeneralizationTest () =
        let coreRepo = CoreMetamodelRepoFactory.Create ()
        let metamodel = coreRepo.InstantiateCoreMetamodel "TestMetamodel"
        let model = coreRepo.InstantiateModel "Model" metamodel

        let parent = metamodel.CreateNode "Parent"
        let child = metamodel.CreateNode "Child"
        metamodel.CreateGeneralization child parent |> ignore

        let node1 = model.InstantiateNode "Node1" child

        node1.IsInstanceOf child |> shouldEqual true

        node1.IsInstanceOf parent |> shouldEqual true

        let node2 = model.InstantiateNode "Node2" parent

        node2.IsInstanceOf child |> shouldEqual false
        node2.IsInstanceOf parent |> shouldEqual true

        let coreElement = coreRepo.CoreMetamodel.Node Consts.element
        let coreNode = coreRepo.CoreMetamodel.Node Consts.node

        parent.IsInstanceOf coreNode |> shouldEqual true
        parent.IsInstanceOf coreElement |> shouldEqual true

        node1.IsInstanceOf coreElement |> shouldEqual true
