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

open Repo.BasicMetamodel.Details.Elements
open Repo.BasicMetamodel

open NUnit.Framework
open FsUnitTyped

[<TestFixture>]
type BasicRepositoryTests() =

    let mutable repo = BasicRepository () :> IBasicRepository

    [<SetUp>]
    member this.SetUp () =
        repo <- BasicRepository () :> IBasicRepository

    [<Test>]
    member this.CreateNodeTest () =
        let node = repo.CreateNode "test"
        node.Name |> shouldEqual "test"
        node.Metatypes |> shouldBeEmpty
        node.OutgoingEdges |> shouldBeEmpty
        ()

    [<Test>]
    member this.CreateEdgeTest () =
        let node1 = repo.CreateNode "node1" :> IBasicElement
        let node2 = repo.CreateNode "node2" :> IBasicElement
        let edge = repo.CreateEdge node1 node2 "edge"

        edge.Source |> shouldEqual node1
        edge.Target |> shouldEqual node2
        edge.TargetName |> shouldEqual "edge"
        edge.Metatypes |> shouldBeEmpty
        edge.OutgoingEdges |> shouldBeEmpty

        ()

    [<Test>]
    member this.ElementsTest () =
        ()

    [<Test>]
    member this.NodesTest () =
        ()

    [<Test>]
    member this.EdgesTest () =
        ()

    [<Test>]
    member this.DeleteElementTest () =
        ()

    [<Test>]
    member this.NodeTest () =
        ()

    [<Test>]
    member this.HasNodeTest () =
        ()

    [<Test>]
    member this.EdgeTest () =
        ()

    [<Test>]
    member this.HasEdgeTest () =
        ()

    [<Test>]
    member this.ClearTest () =
        ()
