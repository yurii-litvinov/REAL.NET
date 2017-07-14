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
open System.Collections.Generic

/// Repository for attribute wrappers. Contains already created wrappers and creates new wrappers if needed.
/// Holds references to attribute wrappers and elements.
type AttributeRepository() =
    let attributes = Dictionary<_, Dictionary<_, _>>()
    member this.GetAttribute (model: DataLayer.IModel) (element: DataLayer.IElement) (name: string) =
        if attributes.ContainsKey element && attributes.[element].ContainsKey name then
            attributes.[element].[name] :> IAttribute
        else
            let newAttribute = Attribute(model, element, name, this)
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
and Attribute(model: DataLayer.IModel, element: DataLayer.IElement, name: string, repository: AttributeRepository) =
    let outgoingLinks node = 
        let outgoingLink: DataLayer.IElement -> bool = 
            function
            | :? DataLayer.IAssociation as a -> a.Source = Some node 
            | _ -> false

        model.Edges
        |> Seq.append model.Metamodel.Edges
        |> Seq.filter outgoingLink 

    let findAssociation element name = 
        model.Edges 
        |> Seq.append model.Metamodel.Edges
        |> Seq.tryFind (function 
                        | :? DataLayer.IAssociation as a -> a.Source = Some element && a.TargetName = name
                        | _ -> false
                       ) 

    let findAssociationOrMetaassociation element name = 
        let result = 
            outgoingLinks element 
            |> Seq.tryFind (function 
                           | :? DataLayer.IAssociation as a -> a.TargetName = name
                           | _ -> false
                           ) 
        
        match result with 
        | Some e -> result
        | None -> findAssociation element.Class name
    
    let attributeNode = 
        let attributeAssociation = findAssociation element name
        let attributeAssociation = 
            match attributeAssociation with
            | Some l -> l
            | None -> 
                // Maybe it is attribute instance, so we need to search for an association that is an instance of required association.
                outgoingLinks element 
                |> Seq.filter (function
                                // TODO: Check association class that it is indeed DataLayer.IAssociation
                                | :? DataLayer.IAssociation as a -> (a.Class :?> DataLayer.IAssociation).TargetName = name
                                | _ -> false
                                )
                // NOTE: Attributes with multiplicity different from 1 are not supported in v1.
                |> Seq.tryHead
                |> Option.defaultWith (fun () -> failwith "Attribute association not found")

        match attributeAssociation.Target with
        | None -> failwith "Attribute association does not have attribute node on other end"
        | Some e -> e

    interface IAttribute with
        member this.Kind = 
            let kindLink = findAssociationOrMetaassociation attributeNode "kind"
            if kindLink.IsNone then
                failwith "Attribute does not have 'kind' link"
            if kindLink.Value.Target.IsNone then
                failwith "'kind' link does not have a node on the other side"
            let kindNode = kindLink.Value.Target.Value
            match kindNode.Name with
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
                match findAssociation attributeNode "stringValue" with
                | None -> attributeNode.Name
                | Some _ -> ""
            and set (v: string): unit = 
                match findAssociation attributeNode "stringValue" with
                | None -> attributeNode.Name <- v
                | Some _ -> failwith "Can not set value for attribute definition"

        member this.Type = null