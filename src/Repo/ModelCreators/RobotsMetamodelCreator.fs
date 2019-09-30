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
//open Repo.InfrastructureMetamodel

///// Initializes repository with Robots Metamodel, first testing metamodel of a real language.
//type RobotsMetamodelCreator() =
//    interface IModelCreator with
//        member this.CreateIn (repo: IDataRepository): unit =
//            let builder = 
//                InfrastructureMetamodel.InfrastructureSemanticsModelBuilder(repo, "RobotsMetamodel")

//            let association = builder.MetamodelNode "Association"

//            let (~+) (name, shape, isAbstract) =
//                let isAbstract = if isAbstract then "true" else "false"
//                builder.InstantiateNode 
//                    name 
//                    "Node" 
//                    ["shape", shape; "isAbstract", isAbstract; "instanceMetatype", "Metatype.Node"]

//            let (--|>) child parent =
//                builder.AddGeneralization child parent

//            let (--->) source (target, targetName, linkName) =
//                let edge = 
//                    builder.InstantiateAssociation 
//                        source 
//                        target 
//                        association
//                        ["shape", "View/Pictures/edge.png"; 
//                            "isAbstract", "false"; 
//                            "instanceMetatype", "Metatype.Edge";
//                            "name", linkName]
                
//                edge.TargetName <- targetName
//                edge

//            let abstractNode = +("AbstractNode", "", true)
//            let initialNode = +("InitialNode", "View/Pictures/initialBlock.png", false)
//            let finalNode = +("FinalNode", "View/Pictures/finalBlock.png", false)

//            let abstractMotorsBlock = +("AbstractMotorsBlock", "", true)
//            builder.AddAttribute abstractMotorsBlock "ports" "M3, M4"

//            let abstractMotorsPowerBlock = +("AbstractMotorsPowerBlock", "", true)
//            builder.AddAttributeWithType abstractMotorsPowerBlock "power" builder.Int "100"

//            let motorsForward = +("MotorsForward", "View/Pictures/enginesForwardBlock.png", false)
//            let motorsBackward = +("MotorsBackward", "View/Pictures/enginesBackwardBlock.png", false)
//            let motorsStop = +("MotorsStop", "View/Pictures/enginesStopBlock.png", false)
//            let timer = +("Timer", "View/Pictures/timerBlock.png", false)

//            let link = abstractNode ---> (abstractNode, "target", "Link")
//            builder.AddAttribute link "guard" ""

//            builder.AddAttributeWithType timer "delay" builder.Int "1000"

//            initialNode --|> abstractNode
//            finalNode --|> abstractNode
//            motorsForward --|> abstractMotorsPowerBlock
//            motorsBackward --|> abstractMotorsPowerBlock
//            abstractMotorsPowerBlock --|> abstractMotorsBlock
//            motorsStop --|> abstractMotorsBlock
//            abstractMotorsBlock --|> abstractNode
//            timer --|> abstractNode

//            ()
