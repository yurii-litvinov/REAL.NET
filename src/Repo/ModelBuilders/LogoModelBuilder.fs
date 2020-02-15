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

            let metamodelInitialNode = Model.findNode metamodel "InitialNode"
            let metamodelFinalNode = Model.findNode metamodel "FinalNode"
           
            let metamodelForward = Model.findNode metamodel "Forward"
            let metamodelBackward = Model.findNode metamodel "Backward"
            let metamodelRight = Model.findNode metamodel "Right"
            let metamodelLeft = Model.findNode metamodel "Left"
            let metamodelPenUp = Model.findNode metamodel "PenUp"
            let metamodelPenDown = Model.findNode metamodel "PenDown"

            let model = repo.CreateModel ("LogoModel", metamodel)  

            /// Creates a link between source and target and returns target
            let (-->) (source: IDataElement) target =
                let aLink = infrastructure.Instantiate model link :?> IAssociation
                aLink.Source <- Some source
                aLink.Target <- Some target
                target

            let createForward distance = 
                let forward = infrastructure.Instantiate model metamodelForward 
                do infrastructure.Element.SetAttributeValue forward "Expression" distance
                forward

            let createBackward distance =
                let backward = infrastructure.Instantiate model metamodelBackward
                do infrastructure.Element.SetAttributeValue backward "Expression" distance
                backward

            let createRight degrees =
                let right = infrastructure.Instantiate model metamodelRight
                infrastructure.Element.SetAttributeValue right "Expression" degrees
                right

            let createLeft degrees =
                let left = infrastructure.Instantiate model metamodelLeft
                infrastructure.Element.SetAttributeValue left "Expression" degrees
                left

            let createPenUp() = infrastructure.Instantiate model metamodelPenUp

            let createPenDown() = infrastructure.Instantiate model metamodelPenDown

            let initialNode = infrastructure.Instantiate model metamodelInitialNode

            let finalNode = infrastructure.Instantiate model metamodelFinalNode

            let forwards = [for i in [1..4] -> createForward "100"]

            let rights = [for i in [1..4] -> createRight "90"]
            
            let left = createLeft "90"

            let backward = createBackward "100"

            initialNode 
            --> forwards.[0] --> rights.[0] --> forwards.[1] --> rights.[1] --> forwards.[2] --> rights.[2]
            --> forwards.[3] --> rights.[3]
            --> left --> backward
            --> finalNode |> ignore

            0 |> ignore