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

/// Initializes repository with test model conforming to Greenhouse Metamodel, actual program that can be written by end-user.
type GreenhouseTestModelBuilder() =
    interface IModelBuilder with
        member this.Build(repo: IRepo): unit =

            let infrastructure = InfrastructureSemantic(repo)
            let metamodel = Repo.findModel repo "GreenhouseMetamodel"

            let metamodelAbstractNode = Model.findNode metamodel "AbstractNode"
            let metamodelAirTemperature = Model.findNode metamodel "AirTemperature"
            let metamodelSoilTemperature = Model.findNode metamodel "SoilTemperature"
            let metamodelInterval = Model.findNode metamodel "Interval"
            let metamodelAndOperation = Model.findNode metamodel "AndOperation"
            let metamodelOpenWindow = Model.findNode metamodel "OpenWindow"

            let link = Model.findAssociationWithSource metamodelAbstractNode "target"

            let model = repo.CreateModel("GreenhouseTestModel", metamodel)
            
            /// Example with "AND" operation
            let airTemperature = infrastructure.Instantiate model metamodelAirTemperature
            let soilTemperature = infrastructure.Instantiate model metamodelSoilTemperature
            let interval1 = infrastructure.Instantiate model metamodelInterval
            let interval2 = infrastructure.Instantiate model metamodelInterval
            let andOperation = infrastructure.Instantiate model metamodelAndOperation
            let openWindow = infrastructure.Instantiate model metamodelOpenWindow

            let (-->) (src: IElement) dst =
                let aLink = infrastructure.Instantiate model link :?> IAssociation
                aLink.Source <- Some src
                aLink.Target <- Some dst
                dst

            airTemperature --> interval1 --> andOperation|> ignore
            soilTemperature --> interval2 --> andOperation |> ignore
            andOperation --> openWindow |> ignore
