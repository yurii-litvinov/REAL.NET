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

namespace Repo.BasicMetamodel.DataObjects.Tests

open Repo.BasicMetamodel.DataObjects
open Repo.BasicMetamodel

open NUnit.Framework
open FsUnitTyped

[<TestFixture>]
type BasicMetamodelEdgeTests() =
    
    let createNode name = BasicMetamodelNode(name) :> IBasicMetamodelElement

    [<SetUp>]
    member this.Setup () =
        ()

    [<Test>]
    member this.NameTest () =
        let node = createNode "node"
        let edge = BasicMetamodelEdge(node, node, "edge") :> IBasicMetamodelEdge
        edge.TargetName |> shouldEqual "edge"
        edge.TargetName <- "newName"
        edge.TargetName |> shouldEqual "newName"
        
        ()

    [<Test>]
    member this.SourceTargetTest () =
        let node1 = createNode "node1"
        let node2 = createNode "node2"
        let node3 = createNode "node3"

        let edge = BasicMetamodelEdge(node1, node2, "edge") :> IBasicMetamodelEdge

        edge.Source |> shouldEqual node1
        edge.Target |> shouldEqual node2

        edge.Target <- node3
        edge.Source |> shouldEqual node1
        edge.Target |> shouldEqual node3

        edge.Source <- node2
        edge.Source |> shouldEqual node2
        edge.Target |> shouldEqual node3

        node1.OutgoingEdges |> shouldBeEmpty
        node2.OutgoingEdges |> shouldContain edge
        node2.OutgoingEdges |> shouldHaveLength 1
        node3.OutgoingEdges |> shouldBeEmpty

        edge.Target <- node2

        edge.Source |> shouldEqual node2
        edge.Target |> shouldEqual node2

        node1.OutgoingEdges |> shouldBeEmpty
        node2.OutgoingEdges |> shouldContain edge
        node2.OutgoingEdges |> shouldHaveLength 1
        node3.OutgoingEdges |> shouldBeEmpty

        ()
