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

/// Initializes repository with Core Metametamodel.
type CoreMetametamodelBuilder() =
    interface IModelBuilder with
        member this.Build(repo: IRepo): unit =
            let model = repo.CreateModel "CoreMetametamodel"

            let node = model.CreateNode("Node", None)

            let (~+) name = model.CreateNode(name, node)

            let element = +"Element"
            let edge = +"Edge"
            let generalization = +"Generalization"
            let association = +"Association"
            let stringNode = +"String"

            let (--|>) (source: IElement) target =
                model.CreateGeneralization(generalization, source, target) |> ignore

            node --|> element
            edge --|> element
            generalization --|> edge
            association --|> edge

            let (--->) (source: IElement) (target, name) =
                model.CreateAssociation(association, source, target, name) |> ignore

            element ---> (element, "class")
            edge ---> (element, "source")
            edge ---> (element, "target")
            association ---> (stringNode, "targetName")

            ()
