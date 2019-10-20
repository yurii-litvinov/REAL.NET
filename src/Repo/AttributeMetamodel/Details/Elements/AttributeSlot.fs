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

type AttributeSlot(node: ICoreNode, pool: AttributePool, repo: ICoreRepository) =
    let attributeMetamodel = repo.Model Consts.attributeMetamodel
    let valueAssociation = (attributeMetamodel.Node Consts.slot).OutgoingAssociation Consts.valueEdge

    let unwrap (element: IAttributeElement) = (element :?> AttributeElement).UnderlyingElement
    
    interface IAttributeSlot with
        member this.Attribute =
            (node.OutgoingAssociation "attribute").Target
            |> pool.WrapAttribute

        /// Returns a node that represents type of an attribute.
        member this.Value
            with get () =
                (node.OutgoingAssociation "value").Target
                |> pool.Wrap
            and set v =
                let oldValue = (node.OutgoingAssociation "value").Target
                oldValue.Model.DeleteElement oldValue
                node.Model.InstantiateAssociation node (unwrap v) valueAssociation |> ignore
