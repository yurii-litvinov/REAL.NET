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

type AttributeAttribute(node: ICoreNode, pool: AttributePool, repo: ICoreRepository) =

    /// Returns underlying Core element for this attribute.
    member this.UnderlyingElement = node :> ICoreElement

    override this.ToString () =
        let this = this :> IAttributeAttribute
        this.Name + ": " + this.Type.ToString ()

    interface IAttributeAttribute with
        /// Returns attribute name.
        member this.Name =
            node.Name

        /// Returns a node that represents type of an attribute.
        member this.Type =
            (node.OutgoingAssociation "type").Target
            |> pool.Wrap
