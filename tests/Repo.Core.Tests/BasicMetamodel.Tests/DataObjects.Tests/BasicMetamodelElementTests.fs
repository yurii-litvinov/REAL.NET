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
type BasicMetamodelElementTests() =

    [<SetUp>]
    member this.Setup () =
        ()

    [<Test>]
    member this.OutgoingEdgesTest () =
        let node1 = BasicMetamodelNode("node1") :> IBasicMetamodelElement
        let node2 = BasicMetamodelNode("node2") :> IBasicMetamodelElement
        let edge1 = BasicMetamodelEdge(node1, node2, "edge1") :> IBasicMetamodelEdge
        let edge2 = BasicMetamodelEdge(node1, node1, "loop") :> IBasicMetamodelEdge

        node1.OutgoingEdges |> shouldHaveLength 2
        node2.OutgoingEdges |> shouldBeEmpty

        node1.OutgoingEdges |> shouldContain edge1
        node1.OutgoingEdges |> shouldContain edge2

        ()
