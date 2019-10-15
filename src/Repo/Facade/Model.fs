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

namespace Repo.Facade

open Repo
open Repo.InfrastructureMetamodel

type Model(model: IInfrastructureModel, pool: FacadePool, repo: IInfrastructureRepository) =
//    /// Initializing infrastructural properties of a model
//    do
//        if not <| model.Properties.ContainsKey "IsVisible" then
//            model.Properties <- model.Properties.Add ("IsVisible", true.ToString())

    let unwrap (element: IElement) = (element :?> Element).UnderlyingElement
    let wrap (element: IInfrastructureElement) = pool.Wrap element

    /// Returns model corresponding to this wrapper in a data layer.
    member this.UnderlyingModel = model

    interface IModel with
        member this.CreateElement (metatype: IElement): IElement =
            let unwrappedMetatype = unwrap metatype :?> IInfrastructureNode
            let name = "a" + metatype.Name
            let element = model.InstantiateNode name unwrappedMetatype Map.empty
            wrap element

        member this.CreateElement (typeName: string): IElement =
            failwith "Not implemented"
            //let ``type`` = (repository.GetModel <| this.UnderlyingModel.OntologicalMetamodel).FindElement typeName
            //(this :> IModel).CreateElement ``type``

        member this.DeleteElement element =
            // TODO: Clean up the memory and check correctness (for example, check for "class" relations)
            failwith "Not implemented"
//            let unwrappedElement = (element :?> Element).UnderlyingElement
//            /// TODO: Delete all attributes.
//            let delete (element: IElement) = 
//                model.MarkElementDeleted unwrappedElement
//                for attribute in element.Attributes do
//                    let unwrappedAttribute = (attribute :?> Attribute).UnderlyingNode
//                    model.MarkElementDeleted unwrappedAttribute
//            delete element

        member this.RestoreElement element = 
            failwith "Not implemented"
//            let unwrappedElement = (element :?> Element).UnderlyingElement
//            let restore (element: IElement) =
//                model.UnmarkDeletedElement unwrappedElement
//                for attribute in element.Attributes do
//                    let unwrappedAttribute = (attribute :?> Attribute).UnderlyingNode
//                    model.UnmarkDeletedElement unwrappedAttribute
//            restore element

        member this.FindElement name =
            failwith "Not implemented"
//            let matchingElements =
//                (this :> IModel).Elements
//                |> Seq.filter (fun e -> e.Name = name)
//                |> Seq.toList

//            match matchingElements with
//            | [e] -> e
//            | [] -> raise (ElementNotFoundException name)
//            | _ -> raise (MultipleElementsException name)

        member this.Nodes =
            failwith "Not implemented"
//            model.Nodes
//            |> Seq.filter infrastructure.Metamodel.IsNode
//            |> Seq.map elementRepository.GetElement
//            |> Seq.cast<INode>

        member this.Edges =
            failwith "Not implemented"
//            model.Edges
//            |> Seq.filter infrastructure.Metamodel.IsAssociation
//            |> Seq.map elementRepository.GetElement
//            |> Seq.cast<IEdge>

        member this.Elements =
            failwith "Not implemented"
//            (this :> IModel).Nodes
//            |> Seq.cast<IElement>
//            |> Seq.append ((this :> IModel).Edges |> Seq.cast<IElement>)

        member this.Metamodel =
            failwith "Not implemented"
//            repository.GetModel model.OntologicalMetamodel

        member this.Name
            with get () = model.Name
            and set v = model.Name <- v

        member this.IsVisible
            with get () = 
                failwith "Not implemented"
//                model.Properties.["IsVisible"] = true.ToString()
            and set v = 
                failwith "Not implemented"
//                model.Properties <- model.Properties.Add ("IsVisible", v.ToString())
