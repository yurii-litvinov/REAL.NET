﻿(* Copyright 2017-2018 REAL.NET group
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License. *)
 
namespace Repo.Metametamodels

open Repo
open Repo.DataLayer

/// Initializes repository with Greenhouse Metamodel
type GreenhouseMetamodelBuilder() =
    interface IModelBuilder with
        member this.Build(repo: IRepo): unit =
            let infrastructure = InfrastructureSemanticLayer.InfrastructureSemantic(repo)
            let metamodel = infrastructure.Metamodel.Model

            let find name = CoreSemanticLayer.Model.findNode metamodel name

            let metamodelElement = find "Element"
            let metamodelNode = find "Node"
            let metamodelGeneralization = find "Generalization"
            let metamodelAssociation = find "Association"

            let model = repo.CreateModel("GreenhouseMetamodel", metamodel)

            let (~+) (name, shape, isAbstract) =
                let node = infrastructure.Instantiate model metamodelNode :?> INode
                node.Name <- name
                infrastructure.Element.SetAttributeValue node "shape" shape
                infrastructure.Element.SetAttributeValue node "isAbstract" (if isAbstract then "true" else "false")
                infrastructure.Element.SetAttributeValue node "instanceMetatype" "Metatype.Node"

                node

            let (--|>) (source: IElement) target =
                model.CreateGeneralization(metamodelGeneralization, source, target) |> ignore

            let (--->) (source: IElement) (target, targetName, linkName) =
                let edge = infrastructure.Instantiate model metamodelAssociation :?> IAssociation
                edge.Source <- Some source
                edge.Target <- Some target
                edge.TargetName <- targetName

                infrastructure.Element.SetAttributeValue edge "shape" "View/Pictures/Greenhouse/Edge.png"
                infrastructure.Element.SetAttributeValue edge "isAbstract" "false"
                infrastructure.Element.SetAttributeValue edge "instanceMetatype" "Metatype.Edge"
                infrastructure.Element.SetAttributeValue edge "name" linkName

                edge

            let abstractNode = +("AbstractNode", "", true)

            let abstractOperation = +("AbstractOperation", "", true)
            let andOperation = +("AndOperation", "View/Pictures/Greenhouse/andOperation.png", false)
            let orOperation = +("OrOperation", "View/Pictures/Greenhouse/orOperation.png", false)

            let abstractActuator = +("AbstractActuator", "", true)
            let openWindow = +("OpenWindow", "View/Pictures/Greenhouse/openWindow.png", false)
            let closeWindow = +("CloseWindow", "View/Pictures/Greenhouse/closeWindow.png", false)
            let pourSoil = +("PourSoil", "View/Pictures/Greenhouse/pourSoil.png", false)

            let abstractSensor = +("AbstractSensor", "", true)
            let airTemperature = +("AirTemperature", "View/Pictures/Greenhouse/airTemperature.png", false)
            let soilTemperature = +("SoilTemperature", "View/Pictures/Greenhouse/soilTemperature.png", false)

            let interval = +("Interval", "View/Pictures/Greenhouse/interval.png", false)

            infrastructure.Element.AddAttribute interval "min" "AttributeKind.Int" "null"
            infrastructure.Element.AddAttribute interval "max" "AttributeKind.Int" "null"
            
            infrastructure.Element.AddAttribute abstractSensor "port" "AttributeKind.Int" "0"
            infrastructure.Element.AddAttribute abstractActuator "port" "AttributeKind.Int" "0"

            let link = abstractNode ---> (abstractNode, "target", "Link")
            infrastructure.Element.AddAttribute link "guard" "AttributeKind.String" ""

            abstractOperation --|> abstractNode
            abstractActuator --|> abstractNode
            abstractSensor --|> abstractNode
            
            openWindow --|> abstractActuator
            closeWindow --|> abstractActuator
            pourSoil --|> abstractActuator

            airTemperature --|> abstractSensor
            soilTemperature --|> abstractSensor

            interval --|> abstractNode

            andOperation --|> abstractOperation
            orOperation --|> abstractOperation

            ()
            

