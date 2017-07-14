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

open System.Collections.Generic
open RepoExperimental

/// Repository for element wrappers. Contains already created wrappers and creates new wrappers if needed.
type ElementRepository(model: DataLayer.IModel, attributeRepository: AttributeRepository) =
    let elements = Dictionary<_, _>()
    interface IElementRepository with
        member this.GetElement (element: DataLayer.IElement) (metatype: Metatype) =
            if elements.ContainsKey element then
                elements.[element]
            else
                let newElement = 
                    match metatype with
                    // TODO: Implement more correctly.
                    | Metatype.Edge -> Edge(model, element :?> DataLayer.IRelationship, this :> IElementRepository, attributeRepository) :> IElement
                    | Metatype.Node -> Node(model, element, this :> IElementRepository, attributeRepository) :> IElement
                    | _ -> failwith "Unknown metatype"
                elements.Add(element, newElement)
                newElement
    
        member this.DeleteElement (element: DataLayer.IElement) =
            if elements.ContainsKey element then
                // TODO: Who will delete attributes?
                elements.Remove(element) |> ignore
