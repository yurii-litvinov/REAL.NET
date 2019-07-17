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

namespace Repo.Metamodels

open Repo.DataLayer
open Repo.CoreMetamodel

/// Initializes repository with Language Metamodel, which is used as a language to define Infrastructure Metamodel,
// which in turn is used to define all other metamodels and closely coupled with editor capabilities.
type LanguageMetamodelBuilder() =
    interface IModelBuilder with
        member this.Build(repo: IDataRepository): unit =
            let coreMetamodel = repo.Model "CoreMetamodel"
            let metamodelNode = coreMetamodel.Node "Node"
            let metamodelGeneralization = coreMetamodel.Node "Generalization"
            let metamodelAssociation = coreMetamodel.Node "Association"

            let languageMetamodel = repo.CreateModel("LanguageMetamodel", coreMetamodel)

            let (~+) name = languageMetamodel.CreateNode(name, metamodelNode)
            let (--|>) (source: IDataElement) target =
                languageMetamodel.CreateGeneralization(metamodelGeneralization, source, target) |> ignore

            let (--->) (source: IDataElement) (target, name) =
                languageMetamodel.CreateAssociation(metamodelAssociation, source, target, name) |> ignore

            let element = +"Element"
            let node = +"Node"
            let edge = +"Edge"
            let generalization = +"Generalization"
            let association = +"Association"
            let stringNode = +"String"
            let enum = +"Enum"

            node --|> element
            edge --|> element
            enum --|> element
            generalization --|> edge
            association --|> edge

            element ---> (element, "class")
            edge ---> (element, "source")
            edge ---> (element, "target")
            association ---> (stringNode, "targetName")
            enum ---> (stringNode, "elements")

            ()
