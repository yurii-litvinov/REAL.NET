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

namespace Repo.FacadeLayer

open Repo

/// Implementation of a node in model.
type Node(repo: DataLayer.IRepo, model: DataLayer.IModel, element: DataLayer.INode, elementRepository: IElementRepository, attributeRepository: AttributeRepository) = 
    inherit Element(repo, model, element, elementRepository, attributeRepository)

    interface INode with
        member this.Name
            with get (): string = element.Name
            and set (v: string): unit =  element.Name <- v
