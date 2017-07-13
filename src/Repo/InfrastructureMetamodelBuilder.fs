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

/// Initializes repository with Infrastructure Metamodel, which is used to define all other metamodels 
/// and closely coupled with editor capabilities.
type InfrastructureMetamodelBuilder() =
    interface IModelBuilder with
        member this.Build(repo: IRepo): unit = 
            let metamodel = repo.Models |> Seq.find (fun m -> m.Name = "LanguageMetamodel")

            let find name =
                metamodel.Nodes |> Seq.find (fun n -> n.Name = name)

            let metamodelNode = find "Node"
            let metamodelGeneralization = find "Generalization"
            let metamodelAssociation = find "Association"
            let metamodelString = find "String"
            let metamodelEnum = find "Enum"
            let metamodelEnumLiteralLink = 
                metamodel.Elements 
                |> Seq.find (function 
                             | :? IAssociation as a -> a.TargetName = "elements" 
                             | _ -> false
                            )

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

            createEnumLiterals metatype ["NodeType"; "EdgeType"]
            createEnumLiterals attributeKind ["String"; "Int"; "Double"; "Boolean"]

            node --|> element
            relationship --|> element
            generalization --|> relationship
            edge --|> relationship

            element ---> (element, "class")
            element ---> (attribute, "attributes")
            attribute ---> (element, "type")
            relationship ---> (element, "from")
            relationship ---> (element, "to")
            edge ---> (stringNode, "targetName")
            repoNode ---> (modelNode, "models")
            modelNode ---> (element, "elements")

            let createAttribute node (``type``: IElement) name = model.CreateAssociation(metamodelAssociation, node, ``type``, name) |> ignore

            createAttribute element stringNode "shape"
            createAttribute element booleanNode "isAbstract"
            createAttribute element metatype "metatype"
            createAttribute element metatype "instanceMetatype"

            createAttribute attribute attributeKind "kind"
            createAttribute attribute stringNode "stringValue"

            createAttribute edge stringNode "targetName"

            ()