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

module CoreMetametamodelBuilderTests

open FsUnit
open NUnit.Framework

open Repo.Metametamodels
open Repo.DataLayer

let init () =
    let repo = DataRepo() :> IRepo
    let builder = CoreMetametamodelBuilder() :> IModelBuilder
    builder.Build repo
    repo

[<Test>]
let ``Builder shall be able to create model in repo`` () =
    let repo = init()

    Seq.length repo.Models |> should equal 1

    (Seq.head repo.Models).Name |> should equal "CoreMetametamodel"

[<Test>]
let ``Every model element shall have correct type`` () =
    let repo = init ()

    Seq.length repo.Models |> should equal 1

    let model = repo.Models |> Seq.head
    let node = model.Nodes |> Seq.find (fun n -> n.Name = "Node") :> IElement
    let generalization = model.Nodes |> Seq.find (fun n -> n.Name = "Generalization") :> IElement
    let association = model.Nodes |> Seq.find (fun n -> n.Name = "Association") :> IElement

    model |> (fun m -> m.Nodes) |> Seq.iter (fun e -> e.Class |> should equal node)
    model |> (fun m -> m.Edges) |> Seq.iter (fun e -> (e.Class = generalization || e.Class = association) |> should be True)
