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

/// Implementation of model.
type InfrastructureModel(model: ILanguageModel, pool: InfrastructurePool, repo: ILanguageRepository) =

    let unwrap (element: IInfrastructureElement) = (element :?> InfrastructureElement).UnderlyingElement
    let wrap element = pool.Wrap element

   /// Returns underlying BasicNode that is a root node for model.
    member this.UnderlyingModel = model

    interface IInfrastructureModel with

        member this.Name 
            with get () = model.Name
            and set v = model.Name <- v

        member this.HasMetamodel =
            failwith "Not implemented"

        member this.Metamodel =
            pool.WrapModel model.Metamodel

        member this.CreateNode name =
            //(this :> IInfrastructureModel).InstantiateNode 
            //    name 
            //    ((pool.Wrap attributeMetamodelNode) :?> IAttributeNode) 
            //    Map.empty
            failwith "Not implemented"

        member this.CreateGeneralization source target =
            wrap <| model.CreateGeneralization (unwrap source) (unwrap target) :?> IInfrastructureGeneralization

        member this.CreateAssociation source target targetName =
            wrap <| model.CreateAssociation (unwrap source) (unwrap target) targetName :?> IInfrastructureAssociation

        member this.InstantiateNode name metatype attributeValues =
            let node = model.InstantiateNode name (unwrap metatype :?> ILanguageNode) Map.empty
            pool.Wrap node :?> IInfrastructureNode

        member this.InstantiateAssociation source target metatype attributeValues =
            let edge = 
                model.InstantiateAssociation 
                    (unwrap source) 
                    (unwrap target) 
                    (unwrap metatype :?> ILanguageAssociation)
                    Map.empty
            wrap edge :?> IInfrastructureAssociation

        member this.Elements = model.Elements |> Seq.map wrap

        member this.Nodes = model.Nodes |> Seq.map wrap |> Seq.cast<IInfrastructureNode>

        member this.Edges = model.Edges |> Seq.map wrap |> Seq.cast<IInfrastructureEdge>

        member this.DeleteElement element =
            model.DeleteElement (unwrap element)

        member this.Node name =
            (this :> IInfrastructureModel).Nodes
            |> Seq.filter (fun n -> n.Name = name)
            |> Helpers.exactlyOneElement name

        member this.HasNode name =
            (this :> IInfrastructureModel).Nodes
            |> Seq.filter (fun n -> n.Name = name)
            |> Seq.isEmpty
            |> not

        member this.Association name =
            (this :> IInfrastructureModel).Edges
            |> Seq.choose (fun e -> if e :? IInfrastructureAssociation then Some (e :?> IInfrastructureAssociation) else None)
            |> Seq.filter (fun e -> e.TargetName = name)
            |> Helpers.exactlyOneElement name

        member this.HasAssociation name =
            (this :> IInfrastructureModel).Edges
            |> Seq.choose (fun e -> if e :? IInfrastructureAssociation then Some (e :?> IInfrastructureAssociation) else None)
            |> Seq.filter (fun e -> e.TargetName = name)
            |> Seq.isEmpty
            |> not

        member this.PrintContents () =
            printfn "Model: %s" model.Name
            ()
