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

/// Initializes repository with Language Metamodel, which is used as a language to define Infrastructure Metamodel, 
// which in turn is used to define all other metamodels and closely coupled with editor capabilities.
type LanguageMetamodelBuilder() =
    interface IModelBuilder with
        member this.Build(repo: IRepo): unit = 
            let coreMetamodel = repo.Models |> Seq.find (fun m -> m.Name = "CoreMetametamodel")
            let metamodelNode = coreMetamodel.Nodes |> Seq.find (fun n -> n.Name = "Node")
            let metamodelGeneralization = coreMetamodel.Nodes |> Seq.find (fun n -> n.Name = "Generalization")
            let metamodelAssociation = coreMetamodel.Nodes |> Seq.find (fun n -> n.Name = "Association")

            let languageMetamodel = repo.CreateModel("LanguageMetamodel", coreMetamodel)

            let (~+) name = languageMetamodel.CreateNode(name, metamodelNode)
            let (--|>) (source: IElement) target = languageMetamodel.CreateGeneralization(metamodelGeneralization, source, target) |> ignore
            let (--->) (source: IElement) (target, name) = languageMetamodel.CreateAssociation(metamodelAssociation, source, target, name) |> ignore

            let element = +"Element"
            let node = +"Node"
            let relationship = +"Relationship"
            let generalization = +"Generalization"
            let association = +"Association"
            let stringNode = +"String"
            let enum = +"Enum"

            node --|> element
            relationship --|> element
            enum --|> element
            generalization --|> relationship
            association --|> relationship

            element ---> (element, "class")
            relationship ---> (element, "source")
            relationship ---> (element, "target")
            association ---> (stringNode, "targetName")
            enum ---> (stringNode, "elements")

            ()