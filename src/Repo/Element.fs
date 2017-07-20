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

open Repo

/// Repository with wrappers for elements (nodes or edges). Contains already created wrappers and creates new wrappers 
/// when needed.
type IElementRepository =
    abstract GetElement: model: DataLayer.IModel -> element: DataLayer.IElement -> IElement
    abstract DeleteElement: model: DataLayer.IModel -> element: DataLayer.IElement -> unit

/// Implementation of an element wrapper.
and [<AbstractClass>] Element
    (
        repo: DataLayer.IRepo
        , model: DataLayer.IModel
        , element: DataLayer.IElement
        , repository: IElementRepository
        , attributeRepository: AttributeRepository
    ) = 

    let findMetatype (element : DataLayer.IElement) =
        if InfrastructureSemanticLayer.InfrastructureMetamodel.isNode repo element then
            Metatype.Node
        elif InfrastructureSemanticLayer.InfrastructureMetamodel.isRelationship repo element then
            Metatype.Edge
        else
            raise (InvalidSemanticOperationException 
                "Trying to get a metatype of an element that is not instance of the Element. Model is malformed.")
       
    member this.UnderlyingElement = element

    interface IElement with
        member this.Attributes = 
            InfrastructureSemanticLayer.Element.attributes repo element |> Seq.map attributeRepository.GetAttribute

        member this.Class = 
            repository.GetElement model element.Class

        member this.IsAbstract =
            if InfrastructureSemanticLayer.InfrastructureMetamodel.isElement repo element then
                if InfrastructureSemanticLayer.InfrastructureMetamodel.isGeneralization repo element then
                    true
                else
                    match InfrastructureSemanticLayer.Element.attributeValue repo element "isAbstract" with
                    | "true" -> true
                    | "false" -> false
                    | _ -> failwith "Incorrect isAbstract attribute value"
            else 
                true

        member this.Shape = InfrastructureSemanticLayer.Element.attributeValue repo element "shape"

        member this.Metatype = findMetatype element

        member this.InstanceMetatype = 
            match InfrastructureSemanticLayer.Element.attributeValue repo element "instanceMetatype" with
            | "Metatype.Node" -> Metatype.Node
            | "Metatype.Edge" -> Metatype.Edge
            | _ -> failwith "Incorrect instanceMetatype attribute value"
