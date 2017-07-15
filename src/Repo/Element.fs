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
open RepoExperimental.CoreSemanticLayer

/// Repository with wrappers for elements (nodes or edges). Contains already created wrappers and creates new wrappers 
/// when needed.
type IElementRepository =
    abstract GetElement: metatype: Metatype -> element: DataLayer.IElement -> IElement
    abstract DeleteElement: element: DataLayer.IElement -> unit

/// Implementation of an element.
and [<AbstractClass>] Element(repo: DataLayer.IRepo, model: DataLayer.IModel, element: DataLayer.IElement, repository: IElementRepository, attributeRepository: AttributeRepository) = 

    let attributes () =
        element.Class
        |> Element.outgoingAssociations repo
        // TODO: Implement searching for attributes more correctly. Attributes may not be direct descendants of Infrastructure Metamodel attributes.
        |> Seq.filter (fun l -> l.Class :? DataLayer.IAssociation && (l.Class :?> DataLayer.IAssociation).TargetName = "attributes")
        |> Seq.map (fun l -> l.Target)
        |> Seq.choose id
        |> Seq.filter (fun e -> e :? DataLayer.INode)
        |> Seq.cast<DataLayer.INode>

    let rec isInstanceOfAssociation targetName (link: DataLayer.IElement) =
        if link.Class :? DataLayer.IAssociation && (link.Class :?> DataLayer.IAssociation).TargetName = targetName then
            true
        else if not (link.Class :? DataLayer.IAssociation) || (link.Class = link) then
            false
        else
            isInstanceOfAssociation targetName link.Class

    let attributeValue name =
        let links = Element.outgoingRelationships repo element
        let attributeLinks = links |> Seq.filter (isInstanceOfAssociation "attributes")
        let interestingAttributeLinks = attributeLinks |> Seq.filter (fun l -> l.Class :? DataLayer.IAssociation && (l.Class :?> DataLayer.IAssociation).TargetName = name)
        let targets = interestingAttributeLinks |> Seq.map (fun l -> (l :?> DataLayer.IAssociation).Target)
        let existingTargets = targets |> Seq.choose id |> Seq.filter (fun e -> e :? DataLayer.INode) |> Seq.cast<DataLayer.INode>
        let names = existingTargets |> Seq.map (fun e -> e.Name)
        // TODO: Implement it correctly.
        let head = names |> Seq.tryHead
        head |> Option.defaultValue ""

    // TODO: Unify it with attributes.
    let metaAttributeValue name =
        let links = Element.outgoingRelationships repo element
        let attributeLinks = links |> Seq.filter (fun l -> l :? DataLayer.IAssociation && (l :?> DataLayer.IAssociation).TargetName = name)
        let targets = attributeLinks |> Seq.map (fun l -> (l :?> DataLayer.IAssociation).Target)
        let existingTargets = targets |> Seq.choose id |> Seq.filter (fun e -> e :? DataLayer.INode) |> Seq.cast<DataLayer.INode>
        let names = existingTargets |> Seq.map (fun e -> e.Name)
        // TODO: Implement it correctly.
        let head = names |> Seq.tryHead
        head |> Option.defaultValue ""
        
    let rec findMetatype (element : DataLayer.IElement) =
        // TODO: Implement it more correctly in Semantic Layer
        if element :? DataLayer.INode && (Node.name element = "Association" || Node.name element = "Generalization") then
            Metatype.Edge
        elif element.Class = element then
            Metatype.Node
        else
            findMetatype element.Class
       
    member this.UnderlyingElement = element

    interface IElement with
        member this.Attributes = 
            attributes () |> Seq.map (fun a -> attributeRepository.GetAttribute a)

        member this.Class = 
            repository.GetElement (findMetatype element.Class) element.Class 

        member this.IsAbstract = 
            match metaAttributeValue "isAbstract" with
            | "true" -> true
            | "false" -> false
            // TODO: Hack, non-nodes do not have this attribute at all.
            | "" -> true
            | _ -> failwith "Incorrect isAbstract attribute value"

        member this.Shape = metaAttributeValue "shape"

        member this.Metatype = 
            findMetatype element

        member this.InstanceMetatype = 
            match metaAttributeValue "instanceMetatype" with
            | "Node" -> Metatype.Node
            | "Edge" -> Metatype.Edge
            | _ -> failwith "Incorrect instanceMetatype attribute value"

