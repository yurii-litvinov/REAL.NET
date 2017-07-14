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

/// Repository with wrappers for elements (nodes or edges). Contains already created wrappers and creates new wrappers when needed.
type IElementRepository =
    abstract GetElement: element: DataLayer.IElement -> metatype: Metatype -> IElement
    abstract DeleteElement: element: DataLayer.IElement -> unit

/// Implementation of an element.
and [<AbstractClass>] Element(model: DataLayer.IModel, element: DataLayer.IElement, repository: IElementRepository, attributeRepository: AttributeRepository) = 
    let outgoingLinks node = 
        let outgoingLink: DataLayer.IElement -> bool = 
            function
            | :? DataLayer.IAssociation as a -> a.Source = Some node 
            | _ -> false

        model.Edges
        |> Seq.append model.Metamodel.Edges
        |> Seq.filter outgoingLink 

    let attributes () =
        element.Class
        |> outgoingLinks
        // TODO: Implement searching for attributes more correctly. Attributes may not be direct descendants of Infrastructure Metamodel attributes.
        |> Seq.filter (fun l -> l.Class :? DataLayer.IAssociation && (l.Class :?> DataLayer.IAssociation).TargetName = "attributes")
        |> Seq.map (fun l -> (l :?> DataLayer.IAssociation).Target)
        |> Seq.choose id
        |> Seq.map (fun e -> e.Name)

    let attributeValue name =
        outgoingLinks element
        |> Seq.filter (fun l -> l.Class.Class :? DataLayer.IAssociation && (l.Class.Class :?> DataLayer.IAssociation).TargetName = "attributes")
        |> Seq.filter (fun l -> l.Class :? DataLayer.IAssociation && (l.Class :?> DataLayer.IAssociation).TargetName = name)
        |> Seq.map (fun l -> (l :?> DataLayer.IAssociation).Target)
        |> Seq.choose id
        |> Seq.map (fun e -> e.Name)
        // TODO: Implement it correctly.
        |> Seq.head
        
    let rec findMetatype (element : DataLayer.IElement) =
        // TODO: Implement it more correctly in Semantic Layer
        if element.Name = "Association" || element.Name = "Generalization" then
            Metatype.Edge
        elif element.Class = element then
            Metatype.Node
        else
            findMetatype element.Class
       
    member this.UnderlyingElement = element

    interface IElement with
        member this.Attributes = 
            attributes ()
            |> Seq.map (fun a -> attributeRepository.GetAttribute model element a)

        member this.Class = 
            repository.GetElement element.Class (findMetatype element.Class)

        member this.IsAbstract = 
            match attributeValue "isAbstract" with
            | "true" -> true
            | "false" -> false
            | _ -> failwith "Incorrect isAbstract attribute value"

        member this.Shape = attributeValue "shape"

        member this.Metatype = 
            findMetatype element

        member this.InstanceMetatype = 
            match attributeValue "instanceMetatype" with
            | "Node" -> Metatype.Node
            | "Edge" -> Metatype.Edge
            | _ -> failwith "Incorrect instanceMetatype attribute value"

