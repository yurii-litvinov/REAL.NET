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

/// Implementation of wrapper factory.
type AttributeFactory(repo: ICoreRepository) =
    interface IAttributeFactory with
        member this.CreateElement element pool =
            match element with
            | :? ICoreNode as n -> AttributeNode(n, pool, repo) :> IAttributeElement
            | :? ICoreGeneralization as g -> AttributeGeneralization(g, pool, repo) :> IAttributeElement
            | :? ICoreInstanceOf as i -> AttributeInstanceOf(i, pool, repo) :> IAttributeElement
            | :? ICoreAssociation as a -> AttributeAssociation(a, pool, repo) :> IAttributeElement
            | _ -> failwith "Unknown subtype"

        member this.CreateModel model pool =
            AttributeModel(model, pool, repo) :> IAttributeModel

        member this.CreateAttribute element pool =
            AttributeAttribute(element :?> ICoreNode, pool, repo) :> IAttributeAttribute

        member this.CreateSlot element pool =
            AttributeSlot(element :?> ICoreNode, pool, repo) :> IAttributeSlot
