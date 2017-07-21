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

open Repo
open Repo.DataLayer
open Repo.InfrastructureSemanticLayer

/// Initializes repository with Robots Metamodel, first testing metamodel of a real language.
type RobotsMetamodelBuilder() =
    interface IModelBuilder with
        member this.Build(repo: IRepo): unit = 
            let elementHelper = InfrastructureSemanticLayer.ElementHelper(repo)
            let metamodel = elementHelper.InfrastructureMetamodel.InfrastructureMetamodel

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

            let model = repo.CreateModel("RobotsMetamodel", metamodel)

            let (~+) (name, shape, isAbstract) = 
                let node = Operations.instantiate repo model metamodelNode :?> INode
                node.Name <- name
                elementHelper.SetAttributeValue node "shape" shape 
                elementHelper.SetAttributeValue node "isAbstract" (if isAbstract then "true" else "false")
                elementHelper.SetAttributeValue node "instanceMetatype" "Metatype.Node" 
                
                node

            let (--|>) (source: IElement) target = 
                model.CreateGeneralization(metamodelGeneralization, source, target) |> ignore

            let (--->) (source: IElement) (target, targetName, linkName) = 
                let edge = Operations.instantiate repo model metamodelAssociation :?> IAssociation
                edge.Source <- Some source
                edge.Target <- Some target
                edge.TargetName <- targetName

                elementHelper.SetAttributeValue edge "shape" "Pictures/Edge.png"
                elementHelper.SetAttributeValue edge "isAbstract" "false"
                elementHelper.SetAttributeValue edge "instanceMetatype" "Metatype.Edge"
                elementHelper.SetAttributeValue edge "name" linkName

                edge

            let abstractNode = +("AbstractNode", "", true)
            let initialNode = +("InitialNode", "Pictures/initialBlock.png", false)
            let finalNode = +("FinalNode", "Pictures/finalBlock.png", false)
            
            let abstractMotorsBlock = +("AbstractMotorsBlock", "", true)
            elementHelper.AddAttribute abstractMotorsBlock "ports" "AttributeKind.String"

            let abstractMotorsPowerBlock = +("AbstractMotorsPowerBlock", "", true)
            elementHelper.AddAttribute abstractMotorsPowerBlock "power" "AttributeKind.Int"

            let motorsForward = +("MotorsForward", "Pictures/enginesForwardBlock.png", false)
            let motorsBackward = +("MotorsBackward", "Pictures/enginesBackwardBlock.png", false)
            let motorsStop = +("MotorsStop", "Pictures/enginesStopBlock.png", false)
            let timer = +("Timer", "Pictures/timerBlock.png", false)

            let link = abstractNode ---> (abstractNode, "target", "Link")
            elementHelper.AddAttribute link "guard" "AttributeKind.String"

            elementHelper.AddAttribute timer "delay" "AttributeKind.Int"

            initialNode --|> abstractNode
            finalNode --|> abstractNode
            motorsForward --|> abstractMotorsPowerBlock
            motorsBackward --|> abstractMotorsPowerBlock
            abstractMotorsPowerBlock --|> abstractMotorsBlock
            motorsStop --|> abstractMotorsBlock
            abstractMotorsBlock --|> abstractNode
            timer --|> abstractNode

            ()
