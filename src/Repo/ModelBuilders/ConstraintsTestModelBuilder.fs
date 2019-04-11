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
open System.Runtime.InteropServices

/// Initializes repository with test model conforming to Robots Metamodel, actual program that can be written by end-user.
type ConstraintsTestModelBuilder() =
    interface IModelBuilder with
        member this.Build(repo: IRepo): unit =
            let infrastructure = InfrastructureSemantic(repo)
            let metamodel = Repo.findModel repo "ConstraintsMetamodel"
            let infrastructureMetamodel = infrastructure.Metamodel.Model

            let metamodelAbstractNode = Model.findNode metamodel "AbstractNode"
            let metamodelInitialNode = Model.findNode metamodel "InitialNode"
            let metamodelFinalNode = Model.findNode metamodel "FinalNode"
            let metamodelMotorsForward = Model.findNode metamodel "MotorsForward"
            let metamodelTimer = Model.findNode metamodel "Timer"

            let metamodelAll = Model.findNode metamodel "AllNodes"
            //let metamodelAny = Model.findNode metamodel "AnyNodes"
            
            let metamodelOr = Model.findNode metamodel "OrNode"
            let metamodelNot = Model.findNode metamodel "NotNode"
            let metamodelNone = Model.findNode metamodel "NoNodes"


            let link = Model.findAssociationWithSource metamodelAbstractNode "target"

            let model = repo.CreateModel("ConstraintsTestModel", metamodel)
            model.Properties <- model.Properties.Add ("IsVisible", false.ToString())
            let initialNode = infrastructure.Instantiate model metamodelInitialNode

            let motorsForw = infrastructure.Instantiate model metamodelMotorsForward
            infrastructure.Element.SetAttributeValue motorsForw "ports" "M3, M4"
            infrastructure.Element.SetAttributeValue motorsForw "power" "10*"


            let (-->) (src: IElement) dst =
                let aLink = infrastructure.Instantiate model link :?> IAssociation
                aLink.Source <- Some src
                aLink.Target <- Some dst
                dst

            
            motorsForw |> ignore
            ()
