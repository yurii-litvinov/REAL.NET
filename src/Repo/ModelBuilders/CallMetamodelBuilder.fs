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
type NodeMetamodelBuilder() =
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

            let model = repo.CreateModel("NodeMetamodel", metamodel)

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

            let abstractNode = +("Node", "", false)
           // let initialNode = +("InitialNode", "View/Pictures/initialBlock.png", false)
            

            ()
            

type ObjectMetamodelBuilder() =
    interface IModelBuilder with
        member this.Build(repo: IRepo): unit =
            let infrastructure = InfrastructureSemanticLayer.InfrastructureSemantic(repo)
            let metamodel = Seq.find (fun (x:IModel) -> x.Name.Equals("NodeMetamodel")) repo.Models

            let find name = CoreSemanticLayer.Model.findNode metamodel name
            let findAssociation node name = CoreSemanticLayer.Model.findAssociationWithSource node name
            
            let metamodelNode = find "Node"
            
            let model = repo.CreateModel("ObjectMetamodel", metamodel) 

            let (~+) (name, shape, isAbstract) =
                let node = infrastructure.Instantiate model metamodelNode :?> INode
                node.Name <- name
                infrastructure.Element.SetAttributeValue node "shape" shape
                infrastructure.Element.SetAttributeValue node "isAbstract" (if isAbstract then "true" else "false")
                infrastructure.Element.SetAttributeValue node "instanceMetatype" "Metatype.Node"

                node
                
            let funcNode = +("FunctionNode", "", false)
            infrastructure.Element.AddAttribute funcNode "params" "AttributeKind.Int" ""
           
           // let initialNode = +("InitialNode", "View/Pictures/initialBlock.png", false)
            

            ()

type ImplObjectMetamodelBuilder() =
    interface IModelBuilder with
        member this.Build(repo: IRepo): unit =
            let infrastructure = InfrastructureSemanticLayer.InfrastructureSemantic(repo)
            let metamodel = Seq.find (fun (x:IModel) -> x.Name.Equals("ObjectMetamodel")) repo.Models

            let find name = CoreSemanticLayer.Model.findNode metamodel name
            let findAssociation node name = CoreSemanticLayer.Model.findAssociationWithSource node name
            
            let metamodelNode = find "FunctionNode"
            
            let model = repo.CreateModel("ImplObjectMetamodel", metamodel) 

            let (~+) (name, shape, isAbstract) =
                let node = infrastructure.Instantiate model metamodelNode :?> INode
                node.Name <- name
                infrastructure.Element.SetAttributeValue node "shape" shape
                infrastructure.Element.SetAttributeValue node "isAbstract" (if isAbstract then "true" else "false")
                infrastructure.Element.SetAttributeValue node "instanceMetatype" "Metatype.Node"

                node
                
            let rotateNode = +("RotateFunctionNode", "View/Pictures/initialBlock.png", false)

            infrastructure.Element.AddAttribute rotateNode "rotation" "AttributeKind.Double" "1.57"
            infrastructure.Element.SetAttributeValue rotateNode "params" "1"
           // let initialNode = +("InitialNode", "View/Pictures/initialBlock.png", false)
            

            ()

type DiagramObjectMetamodelBuilder() =
    interface IModelBuilder with
        member this.Build(repo: IRepo): unit =
            let infrastructure = InfrastructureSemanticLayer.InfrastructureSemantic(repo)
            let metamodel = Seq.find (fun (x:IModel) -> x.Name.Equals("ImplObjectMetamodel")) repo.Models

            let find name = CoreSemanticLayer.Model.findNode metamodel name
            let findAssociation node name = CoreSemanticLayer.Model.findAssociationWithSource node name
            
            let metamodelNode = find "RotateFunctionNode"
            
            let model = repo.CreateModel("DiagramObjectMetamodel", metamodel) 

            let (~+) (name, shape, isAbstract) =
                let node = infrastructure.Instantiate model metamodelNode :?> INode
                node.Name <- name
                infrastructure.Element.SetAttributeValue node "shape" shape
                infrastructure.Element.SetAttributeValue node "isAbstract" (if isAbstract then "true" else "false")
                infrastructure.Element.SetAttributeValue node "instanceMetatype" "Metatype.Node"

                node
                
            let callNode = +("CallFunctionNode", "", false)

            infrastructure.Element.AddAttribute callNode "callParams" "AttributeKind.Int" "2"
           // let initialNode = +("InitialNode", "View/Pictures/initialBlock.png", false)
            

            ()
