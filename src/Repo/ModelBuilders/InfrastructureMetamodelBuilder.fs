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
            let (--|>) (source: IElement) target =
                model.CreateGeneralization(metamodelGeneralization, source, target) |> ignore

            let createEnum name = model.CreateNode(name, metamodelEnum)
            let createEnumLiterals enum literals =
                literals |> Seq.iter (fun l ->
                    let enumLiteral = model.CreateNode(l, metamodelString)
                    model.CreateAssociation(metamodelEnumLiteralLink, enum, enumLiteral, "enumElement") |> ignore
                )

            let element = +"Element"
            let node = +"Node"
            let edge = +"Edge"
            let generalization = +"Generalization"
            let association = +"Association"
            let stringNode = +"String"
            let booleanNode = +"Boolean"
            let attribute = +"Attribute"
            let modelNode = +"Model"
            let repoNode = +"Repo"
            let metatype = createEnum "Metatype"
            let attributeKind = createEnum "AttributeKind"

            let createAttribute node (``type``: IElement) name value =
                let typeNodeToKind = function
                | t when t = stringNode -> "AttributeKind.String"
                | t when t = booleanNode -> "AttributeKind.Boolean"

                /// NOTE: It is actually an enum value, but enums are not supported in v1.
                | t when t = attributeKind -> "AttributeKind.String"
                | t when t = metatype -> "AttributeKind.String"
                | _ -> failwith "unknown type node"

                let attributeNode = +name
                model.CreateAssociation(metamodelAssociation, node, attributeNode, name) |> ignore

                let kindNode = Model.findNode model (typeNodeToKind (``type`` :?> INode))
                model.CreateAssociation(metamodelAssociation, attributeNode, kindNode, "kind") |> ignore

                let valueNode = Model.tryFindNode model value
                if valueNode.IsSome then
                    /// All these attribute are immutable, so can be shared (at least it seems so).
                    model.CreateAssociation(metamodelAssociation, attributeNode, valueNode.Value, "stringValue")
                    |> ignore
                else
                    let stringValueNode = +value
                    model.CreateAssociation(metamodelAssociation, attributeNode, stringValueNode, "stringValue")
                    |> ignore

                // All linguistic attributes shall be instantiable, since every model shall conform to Infrastructure
                // Metamodel if it shall be opened in editor.
                let trueNode = Model.tryFindNode model "true"
                let trueNode = if trueNode.IsNone then +"true" else trueNode.Value

                model.CreateAssociation(metamodelAssociation, attributeNode, trueNode, "isInstantiable") |> ignore

            let (--->) (source: IElement) (target, name, isAbstract) =
                let association = model.CreateAssociation(metamodelAssociation, source, target, name)
                createAttribute association stringNode "shape" "View/Pictures/edge.png"
                createAttribute association booleanNode "isAbstract" (if isAbstract then "true" else "false")
                createAttribute association metatype "instanceMetatype" "Metatype.Edge"

            createEnumLiterals metatype ["Metatype.Node"; "Metatype.Edge"]

            createEnumLiterals attributeKind
                ["AttributeKind.String"
                ; "AttributeKind.Int"
                ; "AttributeKind.Double"
                ; "AttributeKind.Boolean"]

            node --|> element
            edge --|> element
            generalization --|> edge
            association --|> edge

            element ---> (element, "class", true)
            element ---> (attribute, "attributes", true)
            attribute ---> (element, "type", true)
            edge ---> (element, "from", true)
            edge ---> (element, "to", true)
            repoNode ---> (modelNode, "models", true)
            modelNode ---> (element, "elements", true)

            createAttribute element stringNode "shape" "View/Pictures/vertex.png"
            createAttribute element booleanNode "isAbstract" "true"
            createAttribute element metatype "instanceMetatype" "Metatype.Node"

            createAttribute node booleanNode "isAbstract" "false"

            createAttribute edge stringNode "shape" "View/Pictures/edge.png"
            createAttribute edge metatype "instanceMetatype" "Metatype.Edge"

            createAttribute association booleanNode "isAbstract" "false"
            createAttribute association stringNode "name" ""
            createAttribute association stringNode "targetName" ""

            createAttribute generalization booleanNode "isAbstract" "false"
            createAttribute generalization stringNode "targetName" ""

            createAttribute attribute attributeKind "kind" "AttributeKind.String"
            createAttribute attribute stringNode "stringValue" ""
            createAttribute attribute booleanNode "isInstantiable" "false"

            ()
