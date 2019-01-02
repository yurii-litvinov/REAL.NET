(* Copyright 2017-2018 REAL.NET group
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

namespace Repo.Metametamodels

open Repo
open Repo.DataLayer
open Repo.InfrastructureSemanticLayer

/// Initializes repository with AirSim Metamodel
type AirSimMetamodelBuilder() =
    interface IModelBuilder with
        member this.Build(repo: IRepo): unit =
            let infrastructure = InfrastructureSemanticLayer.InfrastructureSemantic(repo)
            let metamodel = infrastructure.Metamodel.Model

            let find name = CoreSemanticLayer.Model.findNode metamodel name
            
            let metamodelNode = find "Node"
            let metamodelGeneralization = find "Generalization"
            let metamodelAssociation = find "Association"
            
            let model = repo.CreateModel("AirSimMetamodel", metamodel)

            let (~+) (name, shape, isAbstract) =
                let node = infrastructure.Instantiate model metamodelNode :?> INode
                node.Name <- name
                infrastructure.Element.SetAttributeValue node "shape" shape
                infrastructure.Element.SetAttributeValue node "isAbstract" (if isAbstract then "true" else "false")
                infrastructure.Element.SetAttributeValue node "instanceMetatype" "Metatype.Node"

                node

            let (--|>) (source: IElement) target =
                model.CreateGeneralization(metamodelGeneralization, source, target) |> ignore

            let (--->) (source: IElement) (target, targetName, linkName) =
                let edge = infrastructure.Instantiate model metamodelAssociation :?> IAssociation
                edge.Source <- Some source
                edge.Target <- Some target
                edge.TargetName <- targetName

                infrastructure.Element.SetAttributeValue edge "shape" "View/Pictures/Edge.png"
                infrastructure.Element.SetAttributeValue edge "name" linkName

                edge
                
            let abstractNode = +("AbstractNode", "", true)
            let initialNode = +("InitialNode", "View/Pictures/initialBlock.png", false)
            let finalNode = +("FinalNode", "View/Pictures/finalBlock.png", false)

            let takeoff = +("Takeoff", "View/Pictures/takeoff.png", false)
            let landing = +("Land", "View/Pictures/land.png", false)
            let move = +("Move", "View/Pictures/move.png", false)
            let hover = +("Hover", "View/Pictures/hover.png", false)
            let timer = +("Timer", "View/Pictures/timerBlock.png", false)
            let ifNode = +("IfNode", "View/Pictures/if.png", false)
            
            let link = abstractNode ---> (abstractNode, "target", "Link")
            let ifLink = abstractNode ---> (abstractNode, "ifTarget", "If Link")
            infrastructure.Element.AddAttribute ifLink "Value" "AttributeKind.Boolean" "true"
            
            infrastructure.Element.AddAttribute takeoff "delay" "AttributeKind.Int" "1"
            infrastructure.Element.AddAttribute move "speed" "AttributeKind.Int" "1"
            infrastructure.Element.AddAttribute timer "delay" "AttributeKind.Int" "1"
            infrastructure.Element.AddAttribute ifNode "condition" "AttributeKind.Boolean" "true"

            initialNode --|> abstractNode
            finalNode --|> abstractNode
            takeoff --|> abstractNode
            landing --|> abstractNode
            move --|> abstractNode
            hover --|> abstractNode
            timer --|> abstractNode
            ifNode --|> abstractNode

            ()