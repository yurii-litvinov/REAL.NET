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
            let (-->) (source: IElement) target =
                let aLink = infrastructure.Instantiate model link :?> IAssociation
                aLink.Source <- Some source
                aLink.Target <- Some target
                target
            
            let initialNode1 = infrastructure.Instantiate model metamodelInitialNode
            let finalNode1 = infrastructure.Instantiate model metamodelFinalNode
            let forward1 = infrastructure.Instantiate model metamodelForward
            let backward1 = infrastructure.Instantiate model metamodelBackward
            let right1 = infrastructure.Instantiate model metamodelRight
            let left1 = infrastructure.Instantiate model metamodelLeft
            let penUp1 = infrastructure.Instantiate model metamodelPenUp
            let penDown1 = infrastructure.Instantiate model metamodelPenDown

            infrastructure.Element.SetAttributeValue forward1 "Expression" "100"
            infrastructure.Element.SetAttributeValue backward1 "Expression" "50"
            infrastructure.Element.SetAttributeValue left1 "Expression" "45"
            infrastructure.Element.SetAttributeValue right1 "Expression" "135"

            initialNode1 --> left1 --> forward1 --> right1 --> backward1 --> finalNode1 |> ignore

            0 |> ignore