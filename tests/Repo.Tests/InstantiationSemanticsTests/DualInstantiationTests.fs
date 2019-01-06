(* Copyright 2019 Yurii Litvinov
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

module DualInstantiationTests

open NUnit.Framework
open FsUnit

open Repo

[<TestFixture>]
type ``Given an initialized repository`` () =
    let repo = RepoFactory.Create ()
    let metamodel = repo.Model "RobotsMetamodel"
    let abstractNodeType = metamodel.FindElement "AbstractNode"

    let model = repo.Model "RobotsTestModel"
    let timerNode = model.FindElement "aTimer"
    let timerType = timerNode.LinguisticType

    [<Test>]
    member this.``Node shall not have linguistic attributes in Attributes list`` () =
        timerNode.Attributes |> Seq.filter (fun attr -> attr.Name = "isAbstract") |> should be Empty

    [<Test>]
    member this.``Linguistic attributes shall still be accessible`` () =
        timerType.Attributes |> Seq.filter (fun attr -> attr.Name = "isAbstract") |> should be Empty
        timerType.IsAbstract |> should be False

        abstractNodeType.IsAbstract |> should be True
