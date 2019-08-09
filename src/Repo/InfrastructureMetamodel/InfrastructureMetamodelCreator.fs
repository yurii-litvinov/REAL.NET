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

namespace Repo.InfrastructureMetamodel

open Repo.DataLayer
open Repo.AttributeMetamodel

/// Initializes repository with Infrastructure Metamodel, which is used to define all other metamodels
/// and closely coupled with editor capabilities.
type InfrastructureMetamodelCreator() =
    interface IModelCreator with
        member this.CreateIn(repo: IDataRepository): unit =
            let metamodel = repo.Model "LanguageMetamodel"
            let builder = AttributeSemanticsModelBuilder(repo, "InfrastructureMetamodel", metamodel)

            let (--->) (source: IDataElement) (target, name) = builder +---> (source, target, name)

            builder.ReinstantiateParentModel ()

            let createEnum name = builder.InstantiateNode name (metamodel.Node "Enum") []

            let createEnumLiterals enum literals =
                literals |> Seq.iter (fun l ->
                    let enumLiteral = builder.AddNode l []
                    let metamodelEnumLiteralLink = ModelSemantics.FindAssociation metamodel "elements"
                    let association = builder.InstantiateAssociation enum enumLiteral metamodelEnumLiteralLink
                    association.TargetName <- "enumElement"
                )

            let booleanNode = builder + "Boolean"
            let modelNode = builder + "Model"
            let repoNode = builder + "Repo"

            let metatype = createEnum "Metatype"
            createEnumLiterals metatype ["Node"; "Edge"]

            let element = builder.Node "Element"

            repoNode ---> (modelNode, "models")
            modelNode ---> (element, "elements")

            builder.AddAttribute element "shape"
            builder.AddAttributeWithType element "isAbstract" booleanNode
            builder.AddAttributeWithType element "instanceMetatype" metatype

            builder.AddAttributeWithType (builder.Node "Node") "isAbstract" booleanNode

            ()
