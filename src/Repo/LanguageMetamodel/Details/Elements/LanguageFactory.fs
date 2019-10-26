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

namespace Repo.LanguageMetamodel.Details.Elements

open Repo
open Repo.LanguageMetamodel
open Repo.AttributeMetamodel

/// Implementation of wrapper factory.
type LanguageFactory(repo: IAttributeRepository) =
    interface ILanguageFactory with
        member this.CreateElement element pool =
            match element with
            | :? IAttributeNode as n -> LanguageNode(n, pool, repo) :> _
            | :? IAttributeGeneralization as g -> LanguageGeneralization(g, pool, repo) :> _
            | :? IAttributeInstanceOf as i -> LanguageInstanceOf(i, pool, repo) :> _
            | :? IAttributeAssociation as a -> LanguageAssociation(a, pool, repo) :> _
            | _ -> failwith "Unknown subtype"

        member this.CreateModel model pool =
            LanguageModel(model, pool, repo) :> _

        member this.CreateEnumeration element pool =
            LanguageEnumeration(element :?> IAttributeNode, pool, repo) :> _

        member this.CreateAttribute attribute pool =
            LanguageAttribute(attribute, pool, repo) :> ILanguageAttribute

        member this.CreateSlot element pool =
            LanguageSlot(element, pool, repo) :> ILanguageSlot
