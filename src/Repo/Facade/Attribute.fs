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

/// Implements attribute wrapper.
type Attribute(attribute: IInfrastructureAttribute, pool: FacadePool, repo: IInfrastructureRepository) =
//    let infrastructureMetamodel = repo.Model InfrastructureMetamodel.Consts.infrastructureMetamodel
//    let dataElementSemantics = InfrastructureMetamodel.Semantics.ElementSemantics(infrastructureMetamodel)

    interface IAttribute with
        member this.Kind =
            failwith "Not implemented"
//            (*
//            let kindNode = dataElementSemantics.Attribute attributeNode "kind"
//            match InfrastructureMetamodel.Semantics.ElementSemantics. kindNode with
//            | "AttributeKind.String" -> AttributeKind.String
//            | "AttributeKind.Int" -> AttributeKind.Int
//            | "AttributeKind.Double" -> AttributeKind.Double
//            | "AttributeKind.Boolean" -> AttributeKind.Boolean
//            | _ -> failwith "unknown 'kind' value"
//            *)
//            AttributeKind.String

        member this.Name = attribute.Name

        member this.StringValue
            with get () =
                failwith "Not implemented"
//                (AttributeMetamodel.Semantics.AttributeSemantics.DefaultValue attributeNode).Name
            and set v =
                failwith "Not implemented"
//                raise (new System.NotImplementedException())

        member this.Type = null

        member this.IsInstantiable = 
            true
//            // dataElementSemantics.AttributeValue attributeNode "isInstantiable" = "true"

        member this.ReferenceValue
            with get() =
                failwith "Not implemented"
            and set v =
                failwith "Not implemented"

