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

/// Tests for basic semantic constraints for repository with Core Metamodel.
[<TestFixture>]
type CoreMetamodelConstraintsTests() =

    let mutable repo = CoreMetamodelRepoFactory.Create ()

    [<SetUp>]
    member this.Setup () =
        repo <- CoreMetamodelRepoFactory.Create ()

    [<Test>]
    member this.``Each element shall be an instance of something, except instanceOf itself`` () =
        let basicRepo = (repo :?> Details.Elements.CoreRepository).UnderlyingRepo

        basicRepo.Elements
        |> Seq.iter (
            function 
            | :? BasicMetamodel.IBasicEdge as e when e.TargetName = "instanceOf" -> 
                e.Metatypes |> Seq.length |> shouldBeSmallerThan 2
            | e -> e.Metatypes |> shouldHaveLength 1
            )
        ()

    [<Test>]
    member this.``There shall be no instanceOf relations between two elements in Core Metamodel`` () =
        let coreMetamodel = repo.Model "CoreMetamodel"
        coreMetamodel.Elements
        |> Seq.iter (fun e -> e.Metatype.IsContainedInSomeModel |> shouldEqual false)

