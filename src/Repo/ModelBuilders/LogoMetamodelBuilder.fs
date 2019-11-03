(* Copyright 2017-2019 REAL.NET group
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
open Repo.CoreSemanticLayer

/// Initializes repository with AirSim Metamodel
type LogoMetamodelBuilder() =
   interface IModelBuilder with
       member this.Build(repo: IRepo): unit =
           let infrastructure = InfrastructureSemanticLayer.InfrastructureSemantic(repo)
           let metamodel = infrastructure.Metamodel.Model

           let find name = CoreSemanticLayer.Model.findNode metamodel name
           
           let metamodelNode = find "Node"
           let metamodelGeneralization = find "Generalization"
           let metamodelAssociation = find "Association"
           
           let model = repo.CreateModel("LogoMetamodel", metamodel)

           /// Creates a node with given name and link to shape
           let (~+) (name, shape, isAbstract) =
               let node = infrastructure.Instantiate model metamodelNode :?> INode
               node.Name <- name
               infrastructure.Element.SetAttributeValue node "shape" shape
               infrastructure.Element.SetAttributeValue node "isAbstract" (if isAbstract then "true" else "false")
               infrastructure.Element.SetAttributeValue node "instanceMetatype" "Metatype.Node"

               node

           /// Creates generalization
           let (--|>) (source: IElement) target =
               model.CreateGeneralization(metamodelGeneralization, source, target) |> ignore
            
           /// Creates an association between source and target    
           let (--->) (source: IElement) (target, targetName, linkName) =
               let edge = infrastructure.Instantiate model metamodelAssociation :?> IAssociation
               edge.Source <- Some source
               edge.Target <- Some target
               edge.TargetName <- targetName

               infrastructure.Element.SetAttributeValue edge "shape" "View/Pictures/Edge.png"
               infrastructure.Element.SetAttributeValue edge "name" linkName

               edge
               
           let abstractNode = +("AbstractNode", "", true)
           let startNode = +("InitialNode", "View/Pictures/initialBlock.png", false)
           let finishNode = +("FinalNode", "View/Pictures/finalBlock.png", false)

           let forward = +("Forward", "", false)
           let backward = +("Backward", "", false)
           let right = +("Right" , "", false)
           let left = +("Left", "", false)           
           
           let penUp = +("PenUp", "", false)
           let penDown = +("PenDown", "", false)

           let link = abstractNode ---> (abstractNode, "target", "Link")

           startNode --|> abstractNode
           finishNode --|> abstractNode           
           forward --|> abstractNode
           backward --|> abstractNode
           right --|> abstractNode
           left --|> abstractNode
           penUp --|> abstractNode
           penDown --|> abstractNode

           infrastructure.Element.AddAttribute forward "Expression" "AttributeKind.String" "0"
           infrastructure.Element.AddAttribute backward "Expression" "AttributeKind.String" "0"
           infrastructure.Element.AddAttribute left "Expression" "AttributeKind.String" "0"
           infrastructure.Element.AddAttribute right "Expression" "AttributeKind.String" "0"

           ()