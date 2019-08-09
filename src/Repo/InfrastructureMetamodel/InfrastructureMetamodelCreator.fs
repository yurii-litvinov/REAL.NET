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

            let createEnum name literals = 
                let enum = builder.InstantiateNode name (metamodel.Node "Enum") []
                literals |> Seq.iter (fun l ->
                    let enumLiteral = builder.AddNode l []
                    let metamodelEnumLiteralLink = ModelSemantics.FindAssociation metamodel "elements"
                    let association = builder.InstantiateAssociation enum enumLiteral metamodelEnumLiteralLink []
                    association.TargetName <- "enumElement"
                )
                enum

            let boolean = createEnum "Boolean" ["true"; "false"]
            let modelNode = builder + "Model"
            let repoNode = builder + "Repo"

            let metatype = createEnum "Metatype" ["Metatype.Node"; "Metatype.Edge"]

            let element = builder.Node "Element"

            repoNode ---> (modelNode, "models")
            modelNode ---> (element, "elements")

            builder.AddAttribute element "shape"
            builder.AddAttributeWithType element boolean (builder.Node "false") "isAbstract"
            builder.AddAttributeWithType element metatype (builder.Node "Node") "instanceMetatype"

            let association = builder.Node "Association"
            builder.AddAttribute association "name"

            ()
