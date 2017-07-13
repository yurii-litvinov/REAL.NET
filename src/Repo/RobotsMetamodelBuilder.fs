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

/// Initializes repository with Robots Metamodel, first testing metamodel of a real language.
type RobotsMetamodelBuilder() =
    interface IModelBuilder with
        member this.Build(repo: IRepo): unit = 
            let metamodel = repo.Models |> Seq.find (fun m -> m.Name = "InfrastructureMetamodel")

            let find name =
                metamodel.Nodes |> Seq.find (fun n -> n.Name = name)

            let findAssociation node name = 
                metamodel.Elements 
                |> Seq.find (function 
                             | :? IAssociation as a -> a.Source = Some node && a.TargetName = name
                             | _ -> false
                            )

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
            let metatypeAssociation = findAssociation metamodelElement "metatype"
            let instanceMetatypeAssociation = findAssociation metamodelElement "instanceMetatype"

            let attributeKindAssociation = findAssociation metamodelAttribute "kind"
            let attributeStringValueAssociation = findAssociation metamodelAttribute "stringValue"

            let edgeTargetNameAssociation = findAssociation metamodelEdge "targetName"

            let model = repo.CreateModel("RobotsMetamodel", metamodel)

            let addAttributeValue node name linkType ``type`` value =
                let value = model.CreateNode(value, ``type``)
                model.CreateAssociation(linkType, node, value, name) |> ignore

            let addAttribute node name kind =
                let attribute = model.CreateNode(name, metamodelAttribute)
                addAttributeValue attribute "kind" attributeKindAssociation metamodelAttributeKindNode kind
                
                // TODO: Differentiate between attributes and slots.
                addAttributeValue attribute "stringValue" attributeStringValueAssociation metamodelStringNode ""

                model.CreateAssociation(attributesAssociation, node, attribute, name) |> ignore

            let (~+) (name, shape, isAbstract, metatype, instanceMetatype) = 
                let node = model.CreateNode(name, metamodelNode)
                
                // TODO: attributes of elementary types require special handling in data layer.
                addAttributeValue node "shape" shapeAssociation metamodelStringNode shape
                addAttributeValue node "isAbstract" isAbstractAssociation metamodelBooleanNode (if isAbstract then "true" else "false")
                addAttributeValue node "metatype" metatypeAssociation metamodelMetatypeNode metatype
                addAttributeValue node "instanceMetatype" instanceMetatypeAssociation metamodelMetatypeNode instanceMetatype

                node

            let (--|>) (source: IElement) target = model.CreateGeneralization(metamodelGeneralization, source, target) |> ignore
            let (--->) (source: IElement) (target, name) = model.CreateAssociation(metamodelEdge, source, target, name)

            let abstractNode = +("AbstractNode", "", true, "Node", "Node")
            let initialNode = +("InitialNode", "Pictures/initialBlock.png", false, "Node", "Node")
            let finalNode = +("FinalNode", "Pictures/finalBlock.png", false, "Node", "Node")
            let motorsForward = +("MotorsForward", "Pictures/enginesForwardBlock.png", false, "Node", "Node")
            let timer = +("Timer", "Pictures/timer.png", false, "Node", "Node")

            // TODO: Elementary types require speial handling on a palette, their values should not appear as nodes on a diagram
            let stringNode = +("String", "", false, "Node", "Node")
            let intNode = +("Int", "", false, "Node", "Node")

            let link = abstractNode ---> (abstractNode, "target")
            addAttribute link "guard" "String"

            addAttribute motorsForward "ports" "String"
            addAttribute motorsForward "power" "Int"

            addAttribute timer "delay" "Int"

            initialNode --|> abstractNode
            finalNode --|> abstractNode
            motorsForward --|> abstractNode
            timer --|> abstractNode

            ()