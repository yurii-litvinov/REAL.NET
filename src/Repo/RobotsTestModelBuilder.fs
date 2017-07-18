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

/// Initializes repository with test model conforming to Robots Metamodel, actual program that can be written by end-user.
type RobotsTestModelBuilder() =
    interface IModelBuilder with
        member this.Build(repo: IRepo): unit = 
            let metamodel = repo.Models |> Seq.find (fun m -> m.Name = "RobotsMetamodel")
            let infrastructureMetamodel = repo.Models |> Seq.find (fun m -> m.Name = "InfrastructureMetamodel")

            let find (model: IModel) name =
                model.Nodes |> Seq.find (fun n -> n.Name = name)

            let findAssociation (model: IModel) node name = 
                model.Edges 
                |> Seq.find (function 
                             | :? IAssociation as a -> a.Source = Some node && a.TargetName = name
                             | _ -> false
                            )

            let metamodelAbstractNode = find metamodel "AbstractNode"
            let metamodelInitialNode = find metamodel "InitialNode"
            let metamodelFinalNode = find metamodel "FinalNode"
            let metamodelMotorsForward = find metamodel "MotorsForward"
            let metamodelTimer = find metamodel "Timer"

            let metamodelMotorsForwardPortsAttribute = find metamodel "ports"
            let metamodelMotorsForwardPowerAttribute = find metamodel "power"
            let metamodelTimerDelayAttribute = find metamodel "delay"

            let metamodelMotorsForwardPortsAssociation = findAssociation metamodel metamodelMotorsForward "ports"
            let metamodelMotorsForwardPowerAssociation = findAssociation metamodel metamodelMotorsForward "power"
            let metamodelTimerDelayAssociation = findAssociation metamodel metamodelTimer "delay"

            let link = findAssociation metamodel metamodelAbstractNode "target"
            let linkGuardAttribute = find metamodel "guard"
            let linkGuardAssociation = findAssociation metamodel link "guard"

            let infrastructureAttribute = find infrastructureMetamodel "Attribute"
            let infrastructureAttributeKind = find infrastructureMetamodel "AttributeKind"

            let infrastructureString = find infrastructureMetamodel "String"

            let infrastructureAttributeKindAssociation = findAssociation infrastructureMetamodel infrastructureAttribute "kind"
            let infrastructureAttributeStringValueAssociation = findAssociation infrastructureMetamodel infrastructureAttribute "stringValue"

            let model = repo.CreateModel("RobotsTestModel", metamodel)

            let initialNode = model.CreateNode("anInitialNode", metamodelInitialNode)
            let finalNode = model.CreateNode("aFinalNode", metamodelFinalNode)

            let motorsForward = model.CreateNode("aMotorsForward", metamodelMotorsForward)
            let timer = model.CreateNode("aTimer", metamodelTimer)

            let addAttributeValue node (attribute: INode) value =
                let attributeAssociation = 
                    metamodel.Edges |> Seq.filter (fun r -> r.Target = Some (attribute :> IElement)) |> Seq.head

                let attributeNode = model.CreateNode(attribute.Name, attribute)
                model.CreateAssociation(attributeAssociation, node, attributeNode, attribute.Name) |> ignore

                let stringValueNode = model.CreateNode(value, infrastructureString)
                model.CreateAssociation(infrastructureAttributeStringValueAssociation, attributeNode, stringValueNode, "stringValue") |> ignore

                let attributeKind = ((findAssociation metamodel attribute "kind").Target.Value :?> INode).Name

                let kindNode = model.CreateNode(attributeKind, infrastructureAttributeKind)
                model.CreateAssociation(infrastructureAttributeKindAssociation, attributeNode, kindNode, "kind") |> ignore

            addAttributeValue motorsForward metamodelMotorsForwardPortsAttribute "M3, M4"
            addAttributeValue motorsForward metamodelMotorsForwardPowerAttribute "100"

            addAttributeValue timer metamodelTimerDelayAttribute "3000"

            let (-->) (src: IElement) dst =
                let link = model.CreateAssociation(link, src, dst, "")
                addAttributeValue link linkGuardAttribute ""
                dst

            initialNode --> motorsForward --> timer --> finalNode |> ignore

            ()