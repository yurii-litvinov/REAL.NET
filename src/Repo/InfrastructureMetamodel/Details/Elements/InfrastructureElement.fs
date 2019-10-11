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

namespace Repo.InfrastructureMetamodel.Details.Elements

open Repo.InfrastructureMetamodel
open Repo.LanguageMetamodel

/// Implementation of Element.
[<AbstractClass>]
type InfrastructureElement(element: ILanguageElement, pool: InfrastructurePool, repo: ILanguageRepository) =

    /// Returns underlying CoreElement.
    member this.UnderlyingElement = element

    interface IInfrastructureElement with

        member this.OutgoingAssociations = 
            failwith "Not implemented"

        member this.IncomingAssociations =
            failwith "Not implemented"

        member this.DirectSupertypes =
            failwith "Not implemented"

        member this.Attributes =
            failwith "Not implemented"

        member this.Slots =
            failwith "Not implemented"

        member this.Model: IInfrastructureModel =
            pool.WrapModel element.Model

        member this.HasMetatype =
            failwith "Not implemented"

        member this.Metatype =
            pool.Wrap element.Metatype
