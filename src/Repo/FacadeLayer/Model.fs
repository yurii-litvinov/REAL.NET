﻿(* Copyright 2020 REAL.NET group
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

///Model repository. Holds all already created wrappers for data models, creates them as needed.
type ModelRepository(infrastructure: InfrastructureSemantic, elementRepository: IElementRepository) =
    let models = Dictionary<DataLayer.IDataModel, Model>()

    member this.GetModel (data: DataLayer.IDataModel) =
        if models.ContainsKey data then
            models.[data] :> IModel
        else
            let newModel = Model(infrastructure, data, elementRepository, this)
            models.Add (data, newModel)
            newModel :> IModel

    member this.DeleteModel (model: IModel) =
        let unwrappedModel = (model :?> Model).UnderlyingModel
        models.Remove unwrappedModel |> ignore

and Model
    (
        infrastructure: InfrastructureSemantic,
        model: DataLayer.IDataModel,
        elementRepository: IElementRepository,
        repository: ModelRepository
    ) =

    let isDeleted (element: DataLayer.IDataElement) = element.IsMarkedDeleted

    /// Initializing infrastructural properties of a model
    do
        if not <| model.Properties.ContainsKey "IsVisible" then
            model.Properties <- model.Properties.Add ("IsVisible", true.ToString())

    /// Returns model corresponding to this wrapper in a data layer.
    member this.UnderlyingModel = model

    interface IModel with
        member this.CreateElement (``type``: IElement) =
            let unwrappedType = (``type`` :?> Element).UnderlyingElement
            let element = infrastructure.Instantiate model unwrappedType
            elementRepository.GetElement element

        member this.CreateElement (typeName: string) =
            let ``type`` = (repository.GetModel <| this.UnderlyingModel.Metamodel).FindElement typeName
            (this :> IModel).CreateElement ``type``

        member this.RemoveElement element =
            // TODO: Clean up the memory and check correctness (for example, check for "class" relations)
            let unwrappedElement = (element :?> Element).UnderlyingElement
            // TODO: Delete all attributes.
            infrastructure.Element.Attributes unwrappedElement
            |> Seq.map (fun x -> x.IsMarkedDeleted <- true) |> ignore
            unwrappedElement.IsMarkedDeleted <- true

        member this.RestoreElement element = 
            let unwrappedElement = (element :?> Element).UnderlyingElement
            // restoring attributes
            infrastructure.Element.Attributes unwrappedElement
            |> Seq.map (fun x -> x.IsMarkedDeleted <- false) |> ignore
            unwrappedElement.IsMarkedDeleted <- false

        member this.FindElement name =
            let matchingElements =
                (this :> IModel).Elements
                |> Seq.filter (fun e -> e.Name = name)
                |> Seq.toList

            match matchingElements with
            | [e] -> e
            | [] -> raise (ElementNotFoundException name)
            | _ -> raise (MultipleElementsException name)

        member this.Nodes =
            model.Nodes
            |> Seq.filter infrastructure.Metamodel.IsNode
            |> Seq.filter (isDeleted >> not)
            |> Seq.map elementRepository.GetElement
            |> Seq.cast<INode>

        member this.Edges =
            model.Edges
            |> Seq.filter infrastructure.Metamodel.IsAssociation
            |> Seq.filter (isDeleted >> not)
            |> Seq.map elementRepository.GetElement
            |> Seq.cast<IEdge>

        member this.Elements =
            (this :> IModel).Nodes
            |> Seq.cast<IElement>
            |> Seq.append ((this :> IModel).Edges |> Seq.cast<IElement>)

        member this.Metamodel =
            repository.GetModel model.Metamodel

        member this.Name
            with get () = model.Name
            and set v = model.Name <- v

        member this.IsVisible
            with get () = 
                model.Properties.["IsVisible"] = true.ToString()
            and set v = 
                model.Properties <- model.Properties.Add ("IsVisible", v.ToString())
