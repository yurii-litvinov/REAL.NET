//(* Copyright 2017 Yurii Litvinov
// *
// * Licensed under the Apache License, Version 2.0 (the "License");
// * you may not use this file except in compliance with the License.
// * You may obtain a copy of the License at
// *
// *     http://www.apache.org/licenses/LICENSE-2.0
// *
// * Unless required by applicable law or agreed to in writing, software
// * distributed under the License is distributed on an "AS IS" BASIS,
// * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// * See the License for the specific language governing permissions and
// * limitations under the License. *)

//namespace Repo.Metamodels

//open Repo
//open Repo.DataLayer
//open Repo.CoreMetamodel

///// Initializes repository with test model conforming to Robots Metamodel, actual program that can be written by end-user.
//type RobotsTestModelCreator() =
//    interface IModelCreator with
//        member this.CreateIn (repo: IDataRepository): unit =
//            let robotsMetamodel = repo.Model "RobotsMetamodel"
//            let builder = 
//                InfrastructureMetamodel.InfrastructureSemanticsModelBuilder(repo, "RobotsTestModel", robotsMetamodel)

//            let metamodelAbstractNode = builder.MetamodelNode "AbstractNode"

//            let link = ModelSemantics.FindAssociationWithSource metamodelAbstractNode "target"

//            let initialNode = builder.InstantiateNode "initialNode" "InitialNode" []
//            let finalNode = builder.InstantiateNode "finalNode" "FinalNode" []
//            let motorsForward = builder.InstantiateNode "motorsForward" "MotorsForward" []

//            let timer = builder.InstantiateNode "timer" "Timer" ["shape", "View/Pictures/timerBlock.png"]

//            let (-->) src dst =
//                builder.InstantiateAssociation src dst link [] |> ignore
//                dst

//            initialNode --> motorsForward --> timer --> finalNode |> ignore

//            ()
