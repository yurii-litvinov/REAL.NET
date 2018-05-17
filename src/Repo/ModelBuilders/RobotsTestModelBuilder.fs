(* Copyright 2017 Yurii Litvinov
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

/// Initializes repository with test model conforming to Robots Metamodel, actual program that can be written by end-user.
type RobotsTestModelBuilder() =
    interface IModelBuilder with
        member this.Build(repo: IRepo): unit =
            let infrastructure = InfrastructureSemantic(repo)
            let metamodel = Repo.findModel repo "RobotsMetamodel"
            let infrastructureMetamodel = infrastructure.Metamodel.Model

            let metamodelAbstractNode = Model.findNode metamodel "AbstractNode"
            //let metamodelInitialNode = Model.findNode metamodel "InitialNode"
            let metamodelFinalNode = Model.findNode metamodel "FinalNode"
            let metamodelMotorsForward = Model.findNode metamodel "MotorsForward"
            let metamodelTimer = Model.findNode metamodel "Timer"

            let link = Model.findAssociationWithSource metamodelAbstractNode "target"

            let model = repo.CreateModel("RobotsTestModel", metamodel)

            //let initialNode = infrastructure.Instantiate model metamodelInitialNode
            let finalNode = infrastructure.Instantiate model metamodelFinalNode

            let motorsForward = infrastructure.Instantiate model metamodelMotorsForward
            infrastructure.Element.SetAttributeValue motorsForward "ports" "M3, M4"
            infrastructure.Element.SetAttributeValue motorsForward "power" "100"

            let timer = infrastructure.Instantiate model metamodelTimer
            infrastructure.Element.SetAttributeValue timer "delay" "3000"

            let (-->) (src: IElement) dst =
                let aLink = infrastructure.Instantiate model link :?> IAssociation
                aLink.Source <- Some src
                aLink.Target <- Some dst
                dst

            //initialNode --> 
            motorsForward --> timer --> finalNode |> ignore

            ()
