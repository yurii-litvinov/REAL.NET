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
            let metamodel = CoreSemanticLayer.Repo.findModel repo "InfrastructureMetamodel"

            let find name = CoreSemanticLayer.Model.findNode metamodel name
            let findAssociation node name = CoreSemanticLayer.Model.findAssociationWithSource metamodel node name

            let metamodelElement = find "Element"
            let metamodelNode = find "Node"
            let metamodelGeneralization = find "Generalization"
            let metamodelEdge = find "Edge"
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

            let edgeTargetNameAssociation = findAssociation metamodelEdge "targetName"

            let model = repo.CreateModel("RobotsMetamodel", metamodel)

            let (~+) (name, shape, isAbstract) = 
                let node = Operations.instantiate repo model metamodelNode :?> INode
                node.Name <- name
                Element.setAttributeValue repo node "shape" shape 
                Element.setAttributeValue repo node "isAbstract" (if isAbstract then "true" else "false")
                Element.setAttributeValue repo node "instanceMetatype" "Metatype.Node" 
                
                node

            let (--|>) (source: IElement) target = model.CreateGeneralization(metamodelGeneralization, source, target) |> ignore

            let (--->) (source: IElement) (target, name) = 
                let edge = Operations.instantiate repo model metamodelEdge :?> IAssociation
                edge.Source <- Some source
                edge.Target <- Some target
                edge.TargetName <- name

                Element.setAttributeValue repo edge "shape" "Pictures/Edge.png"
                Element.setAttributeValue repo edge "isAbstract" "false"
                Element.setAttributeValue repo edge "instanceMetatype" "Metatype.Edge"

                edge

            let abstractNode = +("AbstractNode", "", true)
            let initialNode = +("InitialNode", "Pictures/initialBlock.png", false)
            let finalNode = +("FinalNode", "Pictures/finalBlock.png", false)
            
            let abstractMotorsBlock = +("AbstractMotorsBlock", "", true)
            Element.addAttribute repo abstractMotorsBlock "ports" "AttributeKind.String"

            let abstractMotorsPowerBlock = +("AbstractMotorsPowerBlock", "", true)
            Element.addAttribute repo abstractMotorsPowerBlock "power" "AttributeKind.Int"

            let motorsForward = +("MotorsForward", "Pictures/enginesForwardBlock.png", false)
            let motorsBackward = +("MotorsBackward", "Pictures/enginesBackwardBlock.png", false)
            let motorsStop = +("MotorsStop", "Pictures/enginesStopBlock.png", false)
            let timer = +("Timer", "Pictures/timerBlock.png", false)

            let link = abstractNode ---> (abstractNode, "target")
            Element.addAttribute repo link "guard" "AttributeKind.String"

            Element.addAttribute repo timer "delay" "AttributeKind.Int"

            initialNode --|> abstractNode
            finalNode --|> abstractNode
            motorsForward --|> abstractMotorsPowerBlock
            motorsBackward --|> abstractMotorsPowerBlock
            abstractMotorsPowerBlock --|> abstractMotorsBlock
            motorsStop --|> abstractMotorsBlock
            abstractMotorsBlock --|> abstractNode
            timer --|> abstractNode

            ()