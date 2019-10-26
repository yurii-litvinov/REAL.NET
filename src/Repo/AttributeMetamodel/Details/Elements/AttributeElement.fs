(* Copyright 2019 REAL.NET group
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

namespace Repo.AttributeMetamodel.Details.Elements

open Repo
open Repo.AttributeMetamodel
open Repo.CoreMetamodel

/// Implementation of Element.
[<AbstractClass>]
type AttributeElement(element: ICoreElement, pool: AttributePool, repo: ICoreRepository) =

    let coreMetamodel = repo.Model CoreMetamodel.Consts.coreMetamodel
    let coreAssociation = coreMetamodel.Node CoreMetamodel.Consts.association
    let model () = element.Model

    let attributeMetamodel = repo.Model Consts.attributeMetamodel
    let attributeMetatype = attributeMetamodel.Node Consts.attribute
    let attributesAssociationMetatype = attributeMetamodel.Association Consts.attributesEdge
    let slotsAssociationMetatype = attributeMetamodel.Association Consts.slotsEdge
    let typeAssociationMetatype = attributeMetamodel.Association Consts.typeEdge

    let wrap = pool.Wrap
    let unwrap (element: IAttributeElement) = (element :?> AttributeElement).UnderlyingElement

    let (--->) source (target, metatype) = (model ()).InstantiateAssociation source target metatype |> ignore

    /// Returns underlying CoreElement.
    member this.UnderlyingElement = element

    override this.ToString () = 
        match element with
        | :? ICoreNode as n -> n.Name
        | :? ICoreAssociation as a -> a.TargetName
        | :? ICoreGeneralization -> "generalization"
        | _ -> "unknown"

    interface IAttributeElement with

        member this.OutgoingAssociations =
            element.OutgoingAssociations
            |> Seq.filter (fun a -> a.IsInstanceOf coreAssociation)
            |> Seq.map wrap
            |> Seq.cast<IAttributeAssociation>

        member this.OutgoingAssociation name =
            element.OutgoingAssociation name |> wrap |> (fun a -> a :?> IAttributeAssociation)

        member this.IncomingAssociations =
            element.IncomingEdges
            |> Seq.choose (function | :? ICoreAssociation as a -> Some a | _ -> None)
            |> Seq.filter (fun a -> a.IsInstanceOf coreAssociation)
            |> Seq.map wrap
            |> Seq.cast<IAttributeAssociation>

        member this.DirectSupertypes =
            element.OutgoingEdges
            |> Seq.filter (fun e -> e :? ICoreGeneralization)
            |> Seq.map (fun e -> e.Target)
            |> Seq.map wrap

        member this.Attributes =
            let selfAttributes =
                element.OutgoingAssociations
                |> Seq.filter (fun a -> a.Metatype = (attributesAssociationMetatype :> ICoreElement))
                |> Seq.map (fun a -> a.Target)
                |> Seq.map pool.WrapAttribute

            (this :> IAttributeElement).DirectSupertypes
            |> Seq.map (fun e -> e.Attributes)
            |> Seq.concat
            |> Seq.append selfAttributes

        member this.AddAttribute name ``type`` =
            if (this :> IAttributeElement).Attributes |> Seq.filter (fun a -> a.Name = name) |> Seq.length = 1 then
                raise <| AmbiguousAttributesException(name)

            let attributeNode = (model ()).InstantiateNode name attributeMetatype
            attributeNode ---> (unwrap ``type``, typeAssociationMetatype)
            element ---> (attributeNode, attributesAssociationMetatype)

        member this.Slots =
            element.OutgoingAssociations
            |> Seq.filter (fun a -> a.Metatype = (slotsAssociationMetatype :> ICoreElement))
            |> Seq.map (fun a -> a.Target)
            |> Seq.map pool.WrapSlot

        member this.Slot name =
            (this :> IAttributeElement).Slots 
            |> Seq.filter (fun s -> s.Attribute.Name = name)
            |> Helpers.exactlyOneElement name

        member this.Model: IAttributeModel =
            pool.WrapModel element.Model

        member this.HasMetatype =
            failwith "Not implemented"

        member this.Metatype =
            pool.Wrap element.Metatype
