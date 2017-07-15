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
open RepoExperimental.CoreSemanticLayer

/// Repository for attribute wrappers. Contains already created wrappers and creates new wrappers if needed.
/// Holds references to attribute wrappers and elements.
type AttributeRepository(repo: DataLayer.IRepo) =
    let attributes = Dictionary<_, Dictionary<_, _>>()
    member this.GetAttribute 
            (element: DataLayer.IElement) 
            (name: string) =

        if attributes.ContainsKey element && attributes.[element].ContainsKey name then
            attributes.[element].[name] :> IAttribute
        else
            let newAttribute = Attribute(repo, element, name, this)
            if not <| attributes.ContainsKey element then
                attributes.Add(element, Dictionary<_, _>())
            attributes.[element].Add(name, newAttribute)
            newAttribute :> IAttribute
    
    member this.DeleteAttribute (element: DataLayer.IElement) (name: string) =
        if attributes.ContainsKey element && attributes.[element].ContainsKey name then
            attributes.[element].Remove(name) |> ignore
        if attributes.ContainsKey element && attributes.[element].Count = 0 then
            attributes.Remove(element) |> ignore

/// Implements attribute functionality
and Attribute
    (
        repo: DataLayer.IRepo
        , element: DataLayer.IElement
        , name: string
        , repository: AttributeRepository
    ) =
    
    let attributeNode = 
        Element.attribute repo element name :?> DataLayer.INode

    interface IAttribute with
        member this.Kind = 
            let kindNode = Element.attribute repo attributeNode "kind"
            match Node.name kindNode with
            | "String" -> AttributeKind.String
            | "Int" -> AttributeKind.Int
            | "Double" -> AttributeKind.Double
            | "Boolean" -> AttributeKind.Boolean
            | _ -> failwith "unknown 'kind' value"

        member this.Name = attributeNode.Name

        member this.ReferenceValue
            with get (): IElement = 
                null
            and set (v: IElement): unit = 
                ()

        member this.StringValue
            with get (): string = 
                Node.name <| Element.attribute repo attributeNode "stringValue"
            and set (v: string): unit = 
                (Element.attribute repo attributeNode "stringValue" :?> DataLayer.INode).Name <- v

        member this.Type = null
