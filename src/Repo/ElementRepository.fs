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

open System.Collections.Generic

open Repo
open Repo.InfrastructureSemanticLayer

/// Repository for element wrappers. Contains already created wrappers and creates new wrappers if needed.
type ElementRepository(repo: DataLayer.IRepo, model: DataLayer.IModel, attributeRepository: AttributeRepository) =
    let elements = Dictionary<_, _>()

    let findMetatype (element : DataLayer.IElement) =
        if InfrastructureMetamodel.isNode repo element then
            Metatype.Node
        elif InfrastructureMetamodel.isRelationship repo element then
            Metatype.Edge
        else
            raise (InvalidSemanticOperationException 
                "Trying to get a metatype of an element that is not instance of the Element. Model is malformed.")

    interface IElementRepository with
        member this.GetElement (element: DataLayer.IElement) =
            if elements.ContainsKey element then
                elements.[element]
            else
                let newElement = 
                    match findMetatype element with
                    | Metatype.Edge -> 
                        if not <| element :? DataLayer.IRelationship then
                            raise (InvalidSemanticOperationException 
                                "Element is an instance of Relationship, but is not a relationship in internal \
                                representation. Nodes can not be instances of Relationship.")
                        else 
                            Edge
                                (repo
                                 , model
                                 , element :?> DataLayer.IRelationship
                                 , this :> IElementRepository
                                 , attributeRepository
                                ) :> IElement
                    | Metatype.Node -> 
                        if not <| element :? DataLayer.INode then
                            raise (InvalidSemanticOperationException 
                                "Element is an instance of Node, but is not a node in internal representation. \
                                Theoretically it is possible (when its ontological metatype is node, but linguistic \
                                metatype is edge), but is not used nor supported in v1.")
                        else 
                            Node
                                (repo
                                 , model
                                 , element :?> DataLayer.INode
                                 , this :> IElementRepository
                                 , attributeRepository
                                ) :> IElement
                    | _ -> failwith "Unknown metatype"
                elements.Add(element, newElement)
                newElement
    
        member this.DeleteElement (element: DataLayer.IElement) =
            if elements.ContainsKey element then
                // TODO: Who will delete attributes?
                elements.Remove(element) |> ignore
