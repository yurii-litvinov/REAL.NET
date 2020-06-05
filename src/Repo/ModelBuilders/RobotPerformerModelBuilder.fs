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

open Repo.DataLayer
open Repo.CoreSemanticLayer
open Repo.InfrastructureSemanticLayer

/// Initializes repository with test model conforming to Logo Metamodel, actual program that can be written by end-user.
type RobotPerformerModelBuilder() =
    interface IModelBuilder with
        member this.Build(repo: IRepo): unit =
            let infrastructure = InfrastructureSemantic(repo)
            let metamodel = Repo.findModel repo "RobotPerformerMetamodel"

            let metamodelAbstractNode = Model.findNode metamodel "AbstractNode"

            let link = Model.findAssociationWithSource metamodelAbstractNode "target"
            let taggedLink = Model.findAssociationWithSource metamodelAbstractNode "taggedTarget"

            let metamodelInitialNode = Model.findNode metamodel "InitialNode"
            let metamodelFinalNode = Model.findNode metamodel "FinalNode"
           
            let metamodelForward = Model.findNode metamodel "Forward"
            let metamodelBackward = Model.findNode metamodel "Backward"
            let metamodelRight = Model.findNode metamodel "Right"
            let metamodelLeft = Model.findNode metamodel "Left"
            
            let metamodelRepeat = Model.findNode metamodel "Repeat"
            let metamodelExpression = Model.findNode metamodel "Expression"
            let metamodelIfElse = Model.findNode metamodel "IfElse"

            let model = repo.CreateModel ("RobotPerformerModel", metamodel)  

            /// Creates a link between source and target and returns target.
            let (-->) (source: IDataElement) target =
                let aLink = infrastructure.Instantiate model link :?> IAssociation
                aLink.Source <- Some source
                aLink.Target <- Some target
                target
                
            /// Creates a tagged link between source and target and return it.   
            let (+->) (source: IDataElement) target =
                let aLink = infrastructure.Instantiate model taggedLink :?> IAssociation
                aLink.Source <- Some source
                aLink.Target <- Some target
                aLink

            let createForward distance = 
                let forward = infrastructure.Instantiate model metamodelForward 
                forward

            let createBackward distance =
                let backward = infrastructure.Instantiate model metamodelBackward
                backward

            let createRight degrees =
                let right = infrastructure.Instantiate model metamodelRight
                right

            let createLeft degrees =
                let left = infrastructure.Instantiate model metamodelLeft
                left

            let createRepeat count =
                let repeat = infrastructure.Instantiate model metamodelRepeat
                infrastructure.Element.SetAttributeValue repeat "Count" count
                repeat
                
            let createExpression expr =
                let expression = infrastructure.Instantiate model metamodelExpression
                infrastructure.Element.SetAttributeValue expression "ExpressionValue" expr
                expression
                
            let createIfElse expr =
                let ifElse = infrastructure.Instantiate model metamodelIfElse
                infrastructure.Element.SetAttributeValue ifElse "ExpressionValue" expr
                ifElse
            
            let initialNode = infrastructure.Instantiate model metamodelInitialNode

            let forward1 = createForward()
            
            let forward2 = createForward()
            
            let backward = createBackward()
            
            let right = createRight()
            
            let left = createLeft()
            
            let finalNode = infrastructure.Instantiate model metamodelFinalNode
            
            let ifElse = createIfElse "2 > 1"
            
            initialNode --> ifElse --> forward1 --> right --> forward2 --> left --> backward --> finalNode |> ignore
            ifElse +-> finalNode |> ignore

            0 |> ignore