﻿(* Copyright 2017-2019 REAL.NET group
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
type PerformerBaseMetamodelBuilder() =
   interface IModelBuilder with
       member this.Build(repo: IRepo): unit =
           let infrastructure = InfrastructureSemanticLayer.InfrastructureSemantic(repo)
           let metamodel = infrastructure.Metamodel.Model

           let find name = CoreSemanticLayer.Model.findNode metamodel name
           
           let metamodelNode = find "Node"
           let metamodelGeneralization = find "Generalization"
           let metamodelAssociation = find "Association"
           
           let model = repo.CreateModel("PerformerBaseMetamodel", metamodel)

           /// Creates a node with given name and link to shape
           let (~+) (name, shape, isAbstract) =
               let node = infrastructure.Instantiate model metamodelNode :?> IDataNode
               node.Name <- name
               infrastructure.Element.SetAttributeValue node "shape" shape
               infrastructure.Element.SetAttributeValue node "isAbstract" (if isAbstract then "true" else "false")
               infrastructure.Element.SetAttributeValue node "instanceMetatype" "Metatype.Node"

               node

           /// Creates generalization
           let (--|>) (source: IDataElement) target =
               model.CreateGeneralization(metamodelGeneralization, source, target) |> ignore
            
           /// Creates an association between source and target    
           let (--->) (source: IDataElement) (target, targetName, linkName) =
               let edge = infrastructure.Instantiate model metamodelAssociation :?> IAssociation
               edge.Source <- Some source
               edge.Target <- Some target
               edge.TargetName <- targetName

               infrastructure.Element.SetAttributeValue edge "shape" "View/Pictures/ProgramIcons/Edge.png"
               infrastructure.Element.SetAttributeValue edge "name" linkName

               edge
               
           let abstractNode = +("AbstractNode", "", false)
           let startNode = +("InitialNode", "View/Pictures/ProgramIcons/initialBlock.png", false)
           let finishNode = +("FinalNode", "View/Pictures/ProgramIcons/finalBlock.png", false)

           let repeat = +("Repeat", "View/Pictures/ProgramIcons/repeat.png", false)
           let expression = +("Expression", "View/Pictures/ProgramIcons/functionBlock.png", false)
           let ifElse = +("IfElse", "View/Pictures/ProgramIcons/ifBlock.png", false) 

           let link = abstractNode ---> (abstractNode, "target", "Link")
           let taggedLink = abstractNode ---> (abstractNode, "taggedTarget", "TaggedLink")
           
           let abstractCommand = +("CommandBase", "", false) 

           startNode --|> abstractNode
           finishNode --|> abstractNode           
           repeat --|> abstractNode
           expression --|> abstractNode
           ifElse --|> abstractNode
           abstractCommand --|> abstractNode

           infrastructure.Element.AddAttribute repeat "Count" "AttributeKind.String" "1"
           infrastructure.Element.AddAttribute expression "ExpressionValue" "AttributeKind.String" ""
           infrastructure.Element.AddAttribute ifElse "ExpressionValue" "AttributeKind.String" "true"
           
           infrastructure.Element.AddAttribute taggedLink "Tag" "AttributeKind.String" ""
           ()