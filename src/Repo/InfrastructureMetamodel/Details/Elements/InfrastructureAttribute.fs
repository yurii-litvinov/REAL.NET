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

open Repo
open Repo.InfrastructureMetamodel
open Repo.LanguageMetamodel

/// Implementation of Attribute.
type InfrastructureAttribute(attribute: ILanguageAttribute, pool: InfrastructurePool, repo: ILanguageRepository) =

    let infrastructureMetamodel = repo.Model InfrastructureMetamodel.Consts.infrastructureMetamodel
    let attributeMetatype = infrastructureMetamodel.Node InfrastructureMetamodel.Consts.attribute
    let defaultValueEdge = attributeMetatype.OutgoingAssociation InfrastructureMetamodel.Consts.defaultValueEdge

    /// Returns underlying Attribute element for this attribute.
    member this.UnderlyingAttribute = attribute

    override this.ToString () =
        attribute.ToString ()

    interface IInfrastructureAttribute with
        member this.Name = attribute.Name

        member this.Type = attribute.Type |> pool.Wrap

        member this.DefaultValue =
            failwith "Not implemented"
