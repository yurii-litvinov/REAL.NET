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

open Repo.DataLayer
open Repo.CoreSemanticLayer
open Repo.InfrastructureSemanticLayer

/// Initializes repository with test model conforming to AirSim Metamodel, actual program that can be written by end-user.
type AirSimModelBuilder() =
    interface IModelBuilder with
        member this.Build(repo: IRepo): unit =
            let infrastructure = InfrastructureSemantic(repo)
            let metamodel = Repo.findModel repo "AirSimMetamodel"
            let infrastructureMetamodel = infrastructure.Metamodel.Model

            let metamodelAbstractNode = Model.findNode metamodel "AbstractNode"
            let metamodelInitialNode = Model.findNode metamodel "InitialNode"
            let metamodelFinalNode = Model.findNode metamodel "FinalNode"
            let metamodelTakeoff = Model.findNode metamodel "Takeoff"
            let metamodelLand = Model.findNode metamodel "Land"
            let metamodelMove = Model.findNode metamodel "Move"
            let metamodelTimer = Model.findNode metamodel "Timer"
            let metamodelIf = Model.findNode metamodel "IfNode"

            let link = Model.findAssociationWithSource metamodelAbstractNode "target"
            let ifLink = Model.findAssociationWithSource metamodelAbstractNode "ifTarget"

            let model = repo.CreateModel("AirSimModel", metamodel)

            let initialNode = infrastructure.Instantiate model metamodelInitialNode
            let finalNode = infrastructure.Instantiate model metamodelFinalNode
            let finalNode2 = infrastructure.Instantiate model metamodelFinalNode

            let takeoff = infrastructure.Instantiate model metamodelTakeoff
            
            let landing = infrastructure.Instantiate model metamodelLand
            let move = infrastructure.Instantiate model metamodelMove

            let timer1 = infrastructure.Instantiate model metamodelTimer
            let timer2 = infrastructure.Instantiate model metamodelTimer
            let timer3 = infrastructure.Instantiate model metamodelTimer
            infrastructure.Element.SetAttributeValue timer1 "delay" "1"
            infrastructure.Element.SetAttributeValue timer2 "delay" "1"
            infrastructure.Element.SetAttributeValue timer3 "delay" "1"
            
            let ifNode = infrastructure.Instantiate model metamodelIf

            let (-->) (src: IElement) dst =
                let aLink = infrastructure.Instantiate model link :?> IAssociation
                aLink.Source <- Some src
                aLink.Target <- Some dst
                dst
            
            let (-->>) (src: IElement) dst =
                let aLink = infrastructure.Instantiate model ifLink :?> IAssociation
                infrastructure.Element.SetAttributeValue aLink "Value" "true"
                aLink.Source <- Some src
                aLink.Target <- Some dst
                dst
            
            let (-->>>) (src: IElement) dst =
                let aLink = infrastructure.Instantiate model ifLink :?> IAssociation
                infrastructure.Element.SetAttributeValue aLink "Value" "false"
                aLink.Source <- Some src
                aLink.Target <- Some dst
                dst

            initialNode --> ifNode -->> timer1 --> takeoff --> timer2 --> move --> timer3 --> landing --> finalNode |> ignore
            ifNode -->>> finalNode2 |> ignore
            ()
