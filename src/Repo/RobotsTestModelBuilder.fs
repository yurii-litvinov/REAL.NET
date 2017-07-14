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

namespace RepoExperimental.Metametamodels

open RepoExperimental.DataLayer

/// Initializes repository with test model conforming to Robots Metamodel, actual program that can be written by end-user.
type RobotsTestModelBuilder() =
    interface IModelBuilder with
        member this.Build(repo: IRepo): unit = 
            let metamodel = repo.Models |> Seq.find (fun m -> m.Name = "RobotsMetamodel")

            let find name =
                metamodel.Nodes |> Seq.find (fun n -> n.Name = name)

            let findAssociation node name = 
                metamodel.Elements 
                |> Seq.find (function 
                             | :? IAssociation as a -> a.Source = Some node && a.TargetName = name
                             | _ -> false
                            )

            let metamodelAbstractNode = find "AbstractNode"
            let metamodelInitialNode = find "InitialNode"
            let metamodelFinalNode = find "FinalNode"
            let metamodelMotorsForward = find "MotorsForward"
            let metamodelTimer = find "Timer"

            let metamodelMotorsForwardPortsAttribute = find "ports"
            let metamodelMotorsForwardPowerAttribute = find "power"
            let metamodelTimerDelayAttribute = find "delay"

            let metamodelMotorsForwardPortsAssociation = findAssociation metamodelMotorsForward "ports"
            let metamodelMotorsForwardPowerAssociation = findAssociation metamodelMotorsForward "power"
            let metamodelTimerDelayAssociation = findAssociation metamodelTimer "delay"

            let link = findAssociation metamodelAbstractNode "target"
            let linkGuardAttribute = find "guard"
            let linkGuardAssociation = findAssociation link "guard"

            let model = repo.CreateModel("RobotsTestModel", metamodel)

            let initialNode = model.CreateNode("anInitialNode", metamodelInitialNode)
            let finalNode = model.CreateNode("aFinalNode", metamodelFinalNode)

            let motorsForward = model.CreateNode("aMotorsForward", metamodelMotorsForward)
            let timer = model.CreateNode("aTimer", metamodelTimer)

            let addAttributeValue node attribute associationType value =
                let attributeValueNode = model.CreateNode(value, attribute)
                model.CreateAssociation(associationType, node, attributeValueNode, "") |> ignore

            addAttributeValue motorsForward metamodelMotorsForwardPortsAttribute metamodelMotorsForwardPortsAssociation "M3, M4"
            addAttributeValue motorsForward metamodelMotorsForwardPowerAttribute metamodelMotorsForwardPowerAssociation "100"

            addAttributeValue timer metamodelTimerDelayAttribute metamodelTimerDelayAssociation "3000"

            let (-->) (src: IElement) dst =
                let link = model.CreateAssociation(link, src, dst, "")
                addAttributeValue link linkGuardAttribute linkGuardAssociation ""
                dst

            initialNode --> motorsForward --> timer --> finalNode |> ignore

            ()