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

namespace Repo.BasicMetamodel.Elements.Tests

open Repo
open Repo.BasicMetamodel.Details.Elements
open Repo.BasicMetamodel

open NUnit.Framework
open FsUnitTyped

[<TestFixture>]
type BasicMetamodelElementTests() =

    let node1 = BasicNode("node1") :> IBasicElement
    let node2 = BasicNode("node2") :> IBasicElement
    let node3 = BasicNode("node3") :> IBasicElement
    let edge1 = BasicEdge(node1, node2, "edge1") :> IBasicEdge
    let edge2 = BasicEdge(node1, node1, "loop") :> IBasicEdge
    let edge3 = BasicEdge(node1, node1, "multiple") :> IBasicEdge
    let edge4 = BasicEdge(node1, node2, "multiple") :> IBasicEdge
    let instanceOf1 = BasicEdge(node1, node2, "instanceOf") :> IBasicEdge
    let instanceOf2 = BasicEdge(node3, node1, "instanceOf") :> IBasicEdge
    let instanceOf3 = BasicEdge(node3, node2, "instanceOf") :> IBasicEdge

    [<SetUp>]
    member this.SetUp () =
        ()

    [<Test>]
    member this.OutgoingEdgesTest () =
        node1.OutgoingEdges |> shouldHaveLength 5
        node2.OutgoingEdges |> shouldBeEmpty

        node1.OutgoingEdges |> shouldContain edge1
        node1.OutgoingEdges |> shouldContain edge2
        node1.OutgoingEdges |> shouldContain edge3
        node1.OutgoingEdges |> shouldContain edge4
        node1.OutgoingEdges |> shouldContain instanceOf1

        ()

    [<Test>]
    member this.OutgoingEdgeTest () =
        node1.OutgoingEdge "edge1" |> shouldEqual edge1
        node1.OutgoingEdge "loop" |> shouldEqual edge2

        (fun () -> node1.OutgoingEdge "nonExistent" |> ignore) |> shouldFail<ElementNotFoundException>
        (fun () -> node1.OutgoingEdge "multiple" |> ignore) |> shouldFail<MultipleElementsException>

        ()

    [<Test>]
    member this.HasExactlyOneOutgoingEdgeTest () =
        node1.HasExactlyOneOutgoingEdge "edge1" |> shouldEqual true
        node1.HasExactlyOneOutgoingEdge "loop" |> shouldEqual true
        node1.HasExactlyOneOutgoingEdge "nonExistent" |> shouldEqual false
        node1.HasExactlyOneOutgoingEdge "multiple" |> shouldEqual false

    [<Test>]
    member this.MetatypesTest () =
        node1.Metatypes |> shouldHaveLength 1
        node1.Metatypes |> shouldContain node2

        node2.Metatypes |> shouldBeEmpty

        node3.Metatypes |> shouldHaveLength 2
        node3.Metatypes |> shouldContain node1
        node3.Metatypes |> shouldContain node2

    [<Test>]
    member this.MetatypeTest () =
        node1.Metatype |> shouldEqual node2
        (fun () -> node2.Metatype |> ignore) |> shouldFail<ElementNotFoundException>
        (fun () -> node3.Metatype |> ignore) |> shouldFail<MultipleElementsException>
