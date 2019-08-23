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

module InstantiationTests

open NUnit.Framework
open FsUnit

open Repo
open Repo.InfrastructureMetamodel

[<Test>]
let ``Dual instantiation shall require linguistic attributes`` () =
    let metamodelCreator = InfrastructureSemanticsModelBuilder("RobotsMetamodel")

    let zero = metamodelCreator.InstantiateNode "0" metamodelCreator.Int []
    
    let timerType = 
        metamodelCreator.AddNode "Timer" 
            [
                { Name = "Interval"; Type = metamodelCreator.Int; DefaultValue = zero};
            ]

    let modelCreator = metamodelCreator.CreateInstanceModelBuilder "RobotsModel"

    (fun() -> modelCreator.InstantiateNode "timer" timerType ["Interval", "1000"] |> ignore) |> shouldFail

    ()

[<Test>]
let ``Check that we have no more than one empty string in each model`` () =
    let repo = TestUtils.init [
                                CoreMetamodel.CoreMetamodelCreator(); 
                                AttributeMetamodel.AttributeMetamodelCreator();
                                LanguageMetamodel.LanguageMetamodelCreator();
                                InfrastructureMetametamodelCreator()
                              ]
    repo.Models 
    |> Seq.map (fun m -> m.Nodes)
    |> Seq.map (Seq.map (fun n -> n.Name))
    |> Seq.iter ((should not' (greaterThan 1)) << Seq.length << Seq.filter ((=) ""))

    ()

[<Test>]
let ``Reinstantiation of Infrastructure Metametamodel shall produce something like Infrastructure Metamodel`` () =
    let repo = TestUtils.init [
                                CoreMetamodel.CoreMetamodelCreator(); 
                                AttributeMetamodel.AttributeMetamodelCreator();
                                LanguageMetamodel.LanguageMetamodelCreator();
                                InfrastructureMetametamodelCreator()
                              ]

    let elementSemantics = LanguageMetamodel.ElementSemantics repo
    
    let builder = 
        LanguageMetamodel.LanguageSemanticsModelBuilder
            (
                repo, 
                "TestModel", 
                repo.Model "InfrastructureMetametamodel"
            )

    builder.ReinstantiateParentModel ()

    builder.Model.HasNode "Node" |> should be True
    builder.Node "Node" |> elementSemantics.HasSlot "isAbstract" |> should be True

    ()