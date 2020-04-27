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
type LogoModelBuilder() =
    interface IModelBuilder with
        member this.Build(repo: IRepo): unit =
            let infrastructure = InfrastructureSemantic(repo)
            let metamodel = Repo.findModel repo "LogoMetamodel"

            let metamodelAbstractNode = Model.findNode metamodel "AbstractNode"

            let link = Model.findAssociationWithSource metamodelAbstractNode "target"
            let taggedLink = Model.findAssociationWithSource metamodelAbstractNode "taggedTarget"

            let metamodelInitialNode = Model.findNode metamodel "InitialNode"
            let metamodelFinalNode = Model.findNode metamodel "FinalNode"
           
            let metamodelForward = Model.findNode metamodel "Forward"
            let metamodelBackward = Model.findNode metamodel "Backward"
            let metamodelRight = Model.findNode metamodel "Right"
            let metamodelLeft = Model.findNode metamodel "Left"
            let metamodelPenUp = Model.findNode metamodel "PenUp"
            let metamodelPenDown = Model.findNode metamodel "PenDown"
            
            let metamodelRepeat = Model.findNode metamodel "Repeat"
            let metamodelExpression = Model.findNode metamodel "Expression"

            let model = repo.CreateModel ("LogoModel", metamodel)  

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
                do infrastructure.Element.SetAttributeValue forward "Distance" distance
                forward

            let createBackward distance =
                let backward = infrastructure.Instantiate model metamodelBackward
                do infrastructure.Element.SetAttributeValue backward "Distance" distance
                backward

            let createRight degrees =
                let right = infrastructure.Instantiate model metamodelRight
                infrastructure.Element.SetAttributeValue right "Degrees" degrees
                right

            let createLeft degrees =
                let left = infrastructure.Instantiate model metamodelLeft
                infrastructure.Element.SetAttributeValue left "Degrees" degrees
                left

            let createRepeat count =
                let repeat = infrastructure.Instantiate model metamodelRepeat
                infrastructure.Element.SetAttributeValue repeat "Count" count
                repeat
                
            let createExpression expr =
                let expression = infrastructure.Instantiate model metamodelExpression
                infrastructure.Element.SetAttributeValue expression "ExpressionValue" expr
                expression
            
            let createPenUp() = infrastructure.Instantiate model metamodelPenUp

            let createPenDown() = infrastructure.Instantiate model metamodelPenDown

            let initialNode = infrastructure.Instantiate model metamodelInitialNode

            let finalNode = infrastructure.Instantiate model metamodelFinalNode

            let forwards = [ for _ in [1..4] -> createForward "a" ]

            let rights = [ for _ in [1..4] -> createRight "b" ]
            
            let left = createLeft "b"

            let backward = createBackward "a"
            
            let repeat = createRepeat "2"
            
            let expression = createExpression "a = 100.0; b = 90.0"

            initialNode -->
            expression -->
            repeat --> 
            forwards.[0] --> rights.[0] --> forwards.[1] --> rights.[1] --> forwards.[2] --> rights.[2] --> 
            forwards.[3] --> rights.[3] --> 
            left --> backward --> repeat
            |> ignore

            let exit = repeat +-> finalNode
            infrastructure.Element.SetAttributeValue exit "Tag" "Exit"
            
            0 |> ignore