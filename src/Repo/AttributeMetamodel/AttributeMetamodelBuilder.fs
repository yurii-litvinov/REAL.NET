(* Copyright 2019 Yurii Litvinov
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

namespace Repo.AttributeMetamodel

open Repo.DataLayer
open Repo.CoreMetamodel

/// Initializes repository with Attribute Metamodel.
type AttributeMetamodelBuilder() =
    interface IModelBuilder with
        member this.Build(repo: IDataRepository): unit =
            let model = CoreSemanticsModelCreator(repo, "AttributeMetamodel")
            let (--->) (source: IDataElement) (target, name) = model +---> (source, target, name)

            model.ReinstantiateParentModel ()
            
            let attribute = model + "Attribute"

            model.Node "Element" ---> (attribute, "attributes")
            attribute ---> (model.Node "String", "name")
            attribute ---> (model.Node "Node", "type")
            attribute ---> (model.Node "Node", "value")

            ()
