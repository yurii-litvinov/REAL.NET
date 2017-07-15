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
    let attributes = Dictionary<_, _>()
    member this.GetAttribute (attributeNide: DataLayer.INode) =
        if attributes.ContainsKey attributeNide then
            attributes.[attributeNide] :> IAttribute
        else
            let newAttribute = Attribute(repo, attributeNide, this)
            attributes.Add(attributeNide, newAttribute)
            newAttribute :> IAttribute
    
    member this.DeleteAttribute (node: DataLayer.INode) =
        if attributes.ContainsKey node then
            attributes.Remove(node) |> ignore

/// Implements attribute wrapper.
and Attribute(repo: DataLayer.IRepo, attributeNode: DataLayer.INode, repository: AttributeRepository) =
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
                (Element.attribute repo attributeNode "stringValue").Name <- v

        member this.Type = null
