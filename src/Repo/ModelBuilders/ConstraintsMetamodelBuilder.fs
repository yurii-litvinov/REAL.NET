﻿(* Copyright 2017 Yurii Litvinov
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

open Repo
open Repo.DataLayer
open Repo.InfrastructureSemanticLayer

/// Initializes repository with Robots Metamodel, first testing metamodel of a real language.
type ConstraintsMetamodelBuilder() =
    interface IModelBuilder with
        member this.Build(repo: IRepo): unit =
            let infrastructure = InfrastructureSemanticLayer.InfrastructureSemantic(repo)
            let metamodel = infrastructure.Metamodel.Model

            let find name = CoreSemanticLayer.Model.findNode metamodel name
            let findAssociation node name = CoreSemanticLayer.Model.findAssociationWithSource node name

            let metamodelElement = find "Element"
            let metamodelNode = find "Node"
            let metamodelGeneralization = find "Generalization"
            let metamodelAssociation = find "Association"
            let metamodelAttribute = find "Attribute"

            let metamodelStringNode = find "String"
            let metamodelBooleanNode = find "Boolean"
            let metamodelMetatypeNode = find "Metatype"
            let metamodelAttributeKindNode = find "AttributeKind"

            let attributesAssociation = findAssociation metamodelElement "attributes"

            let shapeAssociation = findAssociation metamodelElement "shape"
            let isAbstractAssociation = findAssociation metamodelElement "isAbstract"
            let instanceMetatypeAssociation = findAssociation metamodelElement "instanceMetatype"

            let attributeKindAssociation = findAssociation metamodelAttribute "kind"
            let attributeStringValueAssociation = findAssociation metamodelAttribute "stringValue"

            let edgeTargetNameAssociation = findAssociation metamodelAssociation "targetName"

            let model = repo.CreateModel("ConstraintsMetamodel", metamodel)
            model.Properties <- model.Properties.Add ("IsVisible", false.ToString())

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

                infrastructure.Element.SetAttributeValue edge "shape" "View/Pictures/edge.png"
                infrastructure.Element.SetAttributeValue edge "isAbstract" "false"
                infrastructure.Element.SetAttributeValue edge "instanceMetatype" "Metatype.Edge"
                infrastructure.Element.SetAttributeValue edge "name" linkName

                edge

            let abstractNode = +("AbstractNode", "", true)
            let initialNode = +("InitialNode", "View/Pictures/initialBlock.png", false)
            let finalNode = +("FinalNode", "View/Pictures/finalBlock.png", false)

            let abstractMotorsBlock = +("AbstractMotorsBlock", "", true)
            infrastructure.Element.AddAttribute abstractMotorsBlock "ports" "AttributeKind.String" "M3, M4"

            let abstractMotorsPowerBlock = +("AbstractMotorsPowerBlock", "", true)
            infrastructure.Element.AddAttribute abstractMotorsPowerBlock "power" "AttributeKind.Int" "100"

            let motorsForward = +("MotorsForward", "View/Pictures/enginesForwardBlock.png", false)
            let motorsBackward = +("MotorsBackward", "View/Pictures/enginesBackwardBlock.png", false)
            let motorsStop = +("MotorsStop", "View/Pictures/enginesStopBlock.png", false)
            let timer = +("Timer", "View/Pictures/timerBlock.png", false)

            let allNodes = +("AllNodes", "View/Pictures/allNodes.png", false)
            //let anyNodes = +("AnyNodes", "View/Pictures/timerBlock.png", false)
            let noNodes = +("NoNodes", "View/Pictures/noNodes.png", false)
            let orNode = +("OrNode", "View/Pictures/orNode.png", false)
            let notNode = +("NotNode", "View/Pictures/notNode.png", false)

            let link = abstractNode ---> (abstractNode, "target", "Link")
            infrastructure.Element.AddAttribute link "guard" "AttributeKind.String" ""

            infrastructure.Element.AddAttribute timer "delay" "AttributeKind.Int" "1000"

            finalNode --|> abstractNode
            motorsForward --|> abstractMotorsPowerBlock
            motorsBackward --|> abstractMotorsPowerBlock
            abstractMotorsPowerBlock --|> abstractMotorsBlock
            motorsStop --|> abstractMotorsBlock
            abstractMotorsBlock --|> abstractNode
            timer --|> abstractNode
            allNodes --|> abstractNode
            //anyNodes --|> abstractNode
            noNodes --|> abstractNode
            //andNode --|> abstractNode
            orNode --|> abstractNode
            notNode --|> abstractNode

            ()
