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

namespace RepoExperimental.FacadeLayer

open RepoExperimental

/// Implementation of edge wrapper.
type Edge(model: DataLayer.IModel, element: DataLayer.IRelationship, elementRepository: IElementRepository, attributeRepository: AttributeRepository) = 
    inherit Element(model, element, elementRepository, attributeRepository)
    interface IEdge with
        member this.From
            with get (): IElement = 
                // TODO: Implement it more correctly.
                elementRepository.GetElement element.Source.Value Metatype.Node
            and set (v: IElement): unit = 
                let dataElement = (v :?> Element).UnderlyingElement
                element.Source <- Some dataElement

        member this.To
            with get (): IElement = 
                // TODO: Implement it more correctly.
                elementRepository.GetElement element.Target.Value Metatype.Node
            and set (v: IElement): unit = 
                let dataElement = (v :?> Element).UnderlyingElement
                element.Target <- Some dataElement
