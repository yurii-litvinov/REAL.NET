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
open Repo.CoreSemanticLayer

/// Repository for attribute wrappers. Contains already created wrappers and creates new wrappers if needed.
/// Holds references to attribute wrappers and elements.
type AttributeRepository() =
    let attributes = Dictionary<_, _>()
    member this.GetAttribute (attributeNode: DataLayer.INode) =
        if attributes.ContainsKey attributeNode then
            attributes.[attributeNode] :> IAttribute
        else
            let newAttribute = Attribute(attributeNode)
            attributes.Add(attributeNode, newAttribute)
            newAttribute :> IAttribute

    member this.DeleteAttribute (node: DataLayer.INode) =
        if attributes.ContainsKey node then
            attributes.Remove(node) |> ignore

/// Implements attribute wrapper.
and Attribute(attributeNode: DataLayer.INode) =

    /// Returns corresponding node from data layer
    member this.UnderlyingNode = attributeNode 

    interface IAttribute with
        member this.Kind =
            let kindNode = Element.attribute attributeNode "kind"
            match Node.name kindNode with
            | "AttributeKind.String" -> AttributeKind.String
            | "AttributeKind.Int" -> AttributeKind.Int
            | "AttributeKind.Double" -> AttributeKind.Double
            | "AttributeKind.Boolean" -> AttributeKind.Boolean
            | _ -> failwith "unknown 'kind' value"

        member this.Name = attributeNode.Name

        member this.ReferenceValue
            with get (): IElement =
                null
            and set (v: IElement): unit =
                ()

        member this.StringValue
            with get (): string =
                Node.name <| Element.attribute attributeNode "stringValue"
            and set (v: string): unit =
                (Element.attribute attributeNode "stringValue").Name <- v

        member this.Type = null

        member this.IsInstantiable = Element.attributeValue attributeNode "isInstantiable" = "true"
