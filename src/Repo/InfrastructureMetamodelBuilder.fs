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

/// Initializes repository with Infrastructure Metamodel, which is used to define all other metamodels 
/// and closely coupled with editor capabilities.
type InfrastructureMetamodelBuilder() =
    interface IModelBuilder with
        member this.Build(repo: IRepo): unit = 
            let metamodel = Repo.findModel repo "LanguageMetamodel"

            let find name = Model.findNode metamodel name

            let metamodelNode = find "Node"
            let metamodelGeneralization = find "Generalization"
            let metamodelAssociation = find "Association"
            let metamodelString = find "String"
            let metamodelEnum = find "Enum"
            let metamodelEnumLiteralLink = Model.findAssociation metamodel "elements"

            let model = repo.CreateModel("InfrastructureMetamodel", metamodel)

            let (~+) name = model.CreateNode(name, metamodelNode)
            let (--|>) (source: IElement) target = model.CreateGeneralization(metamodelGeneralization, source, target) |> ignore
            let (--->) (source: IElement) (target, name) = model.CreateAssociation(metamodelAssociation, source, target, name) |> ignore
            let createEnum name = model.CreateNode(name, metamodelEnum)
            let createEnumLiterals enum literals = 
                literals |> Seq.iter (fun l ->
                    let enumLiteral = model.CreateNode(l, metamodelString)
                    model.CreateAssociation(metamodelEnumLiteralLink, enum, enumLiteral, "") |> ignore
                )

            let element = +"Element"
            let node = +"Node"
            let relationship = +"Relationship"
            let generalization = +"Generalization"
            let edge = +"Edge"
            let stringNode = +"String"
            let booleanNode = +"Boolean"
            let attribute = +"Attribute"
            let modelNode = +"Model"
            let repoNode = +"Repo"
            let metatype = createEnum "Metatype"
            let attributeKind = createEnum "AttributeKind"

            createEnumLiterals metatype ["Metatype.Node"; "Metatype.Edge"]
            
            createEnumLiterals attributeKind 
                ["AttributeKind.String"
                ; "AttributeKind.Int"
                ; "AttributeKind.Double"
                ; "AttributeKind.Boolean"]

            node --|> element
            relationship --|> element
            generalization --|> relationship
            edge --|> relationship

            element ---> (element, "class")
            element ---> (attribute, "attributes")
            attribute ---> (element, "type")
            relationship ---> (element, "from")
            relationship ---> (element, "to")
            repoNode ---> (modelNode, "models")
            modelNode ---> (element, "elements")

            let createAttribute node (``type``: IElement) name value =
                let typeNodeToKind = function
                | t when t = stringNode -> "AttributeKind.String"
                | t when t = booleanNode -> "AttributeKind.Boolean"
                
                /// NOTE: It is actually an enum value, but enums are not supported in v1.
                | t when t = attributeKind -> "AttributeKind.String"
                | t when t = metatype -> "AttributeKind.String"
                | _ -> failwith "unknown type node"

                let attributeNode = +name
                node ---> (attributeNode, name)

                let kindNode = Model.findNode model (typeNodeToKind (``type`` :?> INode))
                attributeNode ---> (kindNode, "kind")

                let valueNode = Model.tryFindNode model value 
                if valueNode.IsSome then
                    attributeNode ---> (valueNode.Value, "stringValue")
                else
                    let stringValueNode = +value
                    attributeNode ---> (stringValueNode, "stringValue")

            createAttribute element stringNode "shape" "Pictures/Vertex.png"
            createAttribute element booleanNode "isAbstract" "true"
            createAttribute element metatype "instanceMetatype" "Metatype.Node"

            createAttribute node booleanNode "isAbstract" "false"

            createAttribute relationship stringNode "shape" "Pictures/Edge.png"
            createAttribute relationship metatype "instanceMetatype" "Metatype.Edge"

            createAttribute edge booleanNode "isAbstract" "false"
            createAttribute edge stringNode "targetName" ""

            createAttribute attribute attributeKind "kind" "AttributeKind.String"
            createAttribute attribute stringNode "stringValue" ""

            ()