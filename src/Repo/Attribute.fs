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
    
    let findAssociation element name = 
        Element.outgoingAssociations repo element
        |> Seq.tryFind (fun a -> a.TargetName = name)

    let findAssociationOrMetaAssociation element name = 
        let result = 
            Element.outgoingAssociations repo element 
            |> Seq.tryFind (fun a -> a.TargetName = name)
        
        match result with 
        | Some e -> result
        | None -> findAssociation element.Class name
    
    let attributeNode = 
        let attributeAssociation = findAssociation element name
        let attributeAssociation = 
            match attributeAssociation with
            | Some l -> l
            | None -> 
                // Maybe it is attribute instance, so we need to search for an association that is an instance 
                // of required association.
                let associationClass = findAssociation element.Class name
                match associationClass with
                | None -> raise (InvalidSemanticOperationException "Attribute association not found")
                | Some l -> let associationInstances = 
                                Element.outgoingAssociations repo element 
                                |> Seq.filter (fun a -> a.Class = (l :> DataLayer.IElement))
                            if Seq.isEmpty associationInstances then
                                raise (InvalidSemanticOperationException <| sprintf "Attribute %s not present" name)
                            elif Seq.length associationInstances <> 1 then
                                raise (MalformedCoreMetamodelException <| sprintf "Attribute %s has multiplicity more than 1" name)
                            else
                                Seq.head associationInstances

        match attributeAssociation.Target with
        | None -> failwith "Attribute association does not have attribute node on other end"
        | Some e -> e :?> DataLayer.INode

    interface IAttribute with
        member this.Kind = 
            let kindLink = findAssociationOrMetaAssociation attributeNode "kind"
            if kindLink.IsNone then
                failwith "Attribute does not have 'kind' link"
            if kindLink.Value.Target.IsNone then
                failwith "'kind' link does not have a node on the other side"
            let kindNode = kindLink.Value.Target.Value :?> DataLayer.INode
            match Node.name kindNode with
            | "String" -> AttributeKind.String
            | "Int" -> AttributeKind.Int
            | "Double" -> AttributeKind.Double
            | "Boolean" -> AttributeKind.Boolean
            | _ -> failwith "unknown 'kind' value"

        member this.Name =
            // TODO: HACK!
            match findAssociation attributeNode "stringValue" with
            | None -> Node.name attributeNode.Class
            | Some _ -> Node.name attributeNode

        member this.ReferenceValue
            with get (): IElement = 
                null
            and set (v: IElement): unit = 
                ()

        member this.StringValue
            with get (): string = 
                match findAssociation attributeNode "stringValue" with
                | None -> Node.name attributeNode
                | Some _ -> ""
            and set (v: string): unit = 
                match findAssociation attributeNode "stringValue" with
                | None -> attributeNode.Name <- v
                | Some _ -> failwith "Can not set value for attribute definition"

        member this.Type = null