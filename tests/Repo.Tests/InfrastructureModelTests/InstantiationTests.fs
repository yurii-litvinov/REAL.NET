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
open Repo.DataLayer

[<Test>]
let ``Instantiation shall not work for abstract nodes`` () =
    let builder = InfrastructureSemanticsModelBuilder("TestModel")

    (fun () -> 
        builder.InstantiationSemantics.InstantiateElement 
            builder.Model 
            "test" 
            (builder.MetamodelNode "Element") 
            Map.empty 
        |> ignore)
    |> should throw typeof<InvalidSemanticOperationException>

[<Test>]
let ``Instantiation shall set slots to default values by default`` () =
    let builder = InfrastructureSemanticsModelBuilder("TestModel")

    let semantics = Semantics.InstantiationSemantics builder.Model.LinguisticMetamodel
    let elementSemantics = Semantics.ElementSemantics builder.Model.LinguisticMetamodel

    let testNode = semantics.InstantiateElement builder.Model "test" (builder.MetamodelNode "Node") Map.empty
    
    elementSemantics.HasSlot "shape" testNode |> should be True
    elementSemantics.StringSlotValue "shape" testNode |> should equal ""
    elementSemantics.HasAttribute "shape" testNode |> should be False

[<Test>]
let ``Instantiation shall use provided slot values`` () =
    let builder = InfrastructureSemanticsModelBuilder("TestModel")

    let semantics = Semantics.InstantiationSemantics builder.Model.LinguisticMetamodel
    let elementSemantics = Semantics.ElementSemantics builder.Model.LinguisticMetamodel

    let testNode = 
        semantics.InstantiateElement
            builder.Model 
            "test" 
            (builder.MetamodelNode "Node") 
            (Map.ofList ["shape", builder.AddStringNode "testShape"])
    
    elementSemantics.HasSlot "shape" testNode |> should be True
    elementSemantics.StringSlotValue "shape" testNode |> should equal "testShape"
    elementSemantics.HasAttribute "shape" testNode |> should be False

[<Test>]
let ``Instantiation shall provide default values for linguistic attributes`` () =
    let metamodelBuilder = InfrastructureSemanticsModelBuilder("RobotsMetamodel")

    let elementSemantics = Semantics.ElementSemantics (metamodelBuilder.Repo.Model Consts.infrastructureMetamodel)

    let zero = metamodelBuilder.AddIntNode "0"
    
    let timerType = 
        metamodelBuilder.AddNode "Timer" 
            [
                { Name = "Interval"; Type = metamodelBuilder.Int; DefaultValue = zero};
            ]

    let modelBuilder = metamodelBuilder.CreateInstanceModelBuilder "RobotsModel"

    let timerInstance = modelBuilder.InstantiateNode "timer" "Timer" ["Interval", "1000"]

    elementSemantics.HasSlot "Interval" timerInstance |> should be True
    elementSemantics.StringSlotValue "Interval" timerInstance |> should equal "1000"
    elementSemantics.HasAttribute "Interval" timerInstance |> should be False
    
    elementSemantics.HasSlot "isAbstract" timerInstance |> should be True
    elementSemantics.StringSlotValue "isAbstract" timerInstance |> should equal "false"
    elementSemantics.HasAttribute "isAbstract" timerInstance |> should be False

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

[<Test>]
let ``Reinstantiation of Infrastructure Metametamodel shall produce something like Infrastructure Metamodel`` () =
    let repo = TestUtils.init [
                                CoreMetamodel.CoreMetamodelCreator(); 
                                AttributeMetamodel.AttributeMetamodelCreator();
                                LanguageMetamodel.LanguageMetamodelCreator();
                                InfrastructureMetametamodelCreator()
                              ]

    let elementSemantics = Semantics.ElementSemantics (repo.Model Consts.infrastructureMetametamodel)

    let infrastructureMetametamodel = repo.Model Consts.infrastructureMetametamodel

    infrastructureMetametamodel.Elements 
    |> Seq.iter 
        (fun (n: IDataElement) -> n.LinguisticType.Model |> should equal (repo.Model "LanguageMetamodel"))

    let testModel = repo.CreateModel("TestModel", infrastructureMetametamodel, infrastructureMetametamodel)
    
    Reinstantiator.reinstantiateInfrastructureMetametamodel repo testModel

    let modelSemantics = 
        Repo.InfrastructureMetamodel.Semantics.ModelSemantics (repo.Model Consts.infrastructureMetametamodel)

    modelSemantics.PrintContents testModel

    testModel.Elements 
    |> Seq.iter 
        (fun (n: IDataElement) -> n.LinguisticType.Model |> should equal infrastructureMetametamodel)

    testModel.HasNode "Node" |> should be True
    testModel.Node "Node" |> elementSemantics.HasSlot "isAbstract" |> should be True
    testModel.Node "Node" |> elementSemantics.HasAttribute "isAbstract" |> should be True

    testModel.Node "Int" |> elementSemantics.HasSlot "isAbstract" |> should be True
    testModel.Node "Int" |> elementSemantics.HasAttribute "isAbstract" |> should be False

    testModel.HasNode "Boolean" |> should be True
    testModel.HasNode "true" |> should be True

    CoreMetamodel.ElementSemantics.HasOutgoingAssociation (testModel.Node "Edge") "source" |> should be True