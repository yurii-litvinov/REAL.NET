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


open Repo.AttributeMetamodel
open Repo.CoreMetamodel

/// Implementation of Element.
[<AbstractClass>]
type AttributeElement(element: ICoreElement, pool: AttributePool, repo: ICoreRepository) =

    /// Returns underlying CoreElement.
    member this.UnderlyingElement = element

    interface IAttributeElement with

        member this.OutgoingAssociations =
            element.OutgoingEdges
            |> Seq.choose(function | :? ICoreAssociation as a -> Some a | _ -> None)
            |> Seq.map pool.Wrap
            |> Seq.cast<IAttributeAssociation>

        member this.IncomingAssociations =
            failwith "Not implemented"

        member this.DirectSupertypes =
            failwith "Not implemented"

        member this.Attributes =
            failwith "Not implemented"

        member this.Slots =
            failwith "Not implemented"

        member this.Model: IAttributeModel =
            pool.WrapModel element.Model

        member this.HasMetatype =
            failwith "Not implemented"

        member this.Metatype =
            pool.Wrap element.Metatype
