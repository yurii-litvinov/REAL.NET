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

module DataElementTests

open NUnit.Framework
open FsUnit

open Repo.DataLayer

[<Test>]
let ``DataElement shall have class and name`` () =
    let model = new DataModel("") :> IModel
    let node = DataNode("node1", None, model) :> INode
    node.Name |> should equal "node1"

    node.Name <- "changedName"
    node.Name |> should equal "changedName"

    node.Class |> should equal node

    let node2 = DataNode("node2", node, model) :> INode

    node2.Class |> should equal node
