//(* Copyright 2017 Yurii Litvinov
// *
// * Licensed under the Apache License, Version 2.0 (the "License");
// * you may not use this file except in compliance with the License.
// * You may obtain a copy of the License at
// *
// *     http://www.apache.org/licenses/LICENSE-2.0
// *
// * Unless required by applicable law or agreed to in writing, software
// * distributed under the License is distributed on an "AS IS" BASIS,
// * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// * See the License for the specific language governing permissions and
// * limitations under the License. *)

//namespace Repo.FacadeLayer

//open System.Collections.Generic

//open Repo

///// Repository for attribute wrappers. Contains already created wrappers and creates new wrappers if needed.
///// Holds references to attribute wrappers and elements.
//type AttributeRepository(repo: DataLayer.IDataRepository) =
//    let attributes = Dictionary<_, _>()
//    member this.GetAttribute (attributeNode: DataLayer.IDataNode) =
//        if attributes.ContainsKey attributeNode then
//            attributes.[attributeNode] :> IAttribute
//        else
//            let newAttribute = Attribute(attributeNode, repo)
//            attributes.Add(attributeNode, newAttribute)
//            newAttribute :> IAttribute

//    member this.DeleteAttribute (node: DataLayer.IDataNode) =
//        if attributes.ContainsKey node then
//            attributes.Remove(node) |> ignore

///// Implements attribute wrapper.
//and Attribute(attributeNode: DataLayer.IDataNode, repo: DataLayer.IDataRepository) =
//    let infrastructureMetamodel = repo.Model InfrastructureMetamodel.Consts.infrastructureMetamodel
//    let dataElementSemantics = InfrastructureMetamodel.Semantics.ElementSemantics(infrastructureMetamodel)

//    interface IAttribute with
//        member this.Kind =
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

//        member this.Name = attributeNode.Name

//        member this.StringDefaultValue
//            with get (): string =
//                (AttributeMetamodel.Semantics.AttributeSemantics.DefaultValue attributeNode).Name
//            and set (v: string): unit =
//                raise (new System.NotImplementedException())

//        member this.Type = null

//        member this.IsInstantiable = 
//            true
//            // dataElementSemantics.AttributeValue attributeNode "isInstantiable" = "true"
