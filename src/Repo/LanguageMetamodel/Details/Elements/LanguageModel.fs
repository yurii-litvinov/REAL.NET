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

/// Implementation of model.
type LanguageModel(model: IAttributeModel, pool: LanguagePool, repo: IAttributeRepository) =

    let unwrap (element: ILanguageElement) = (element :?> LanguageElement).UnderlyingElement
    let wrap element = pool.Wrap element

    let attributeMetamodel = repo.Model AttributeMetamodel.Consts.attributeMetamodel
    let languageModel = repo.Model LanguageMetamodel.Consts.languageMetamodel

    let attributeMetamodelNode = attributeMetamodel.Node AttributeMetamodel.Consts.node
    
    let enumerationNode = languageModel.Node LanguageMetamodel.Consts.enumeration
    let stringNode = languageModel.Node LanguageMetamodel.Consts.string

    let enumElementsAssociation = enumerationNode.OutgoingAssociation LanguageMetamodel.Consts.elementsEdge 

   /// Returns underlying BasicNode that is a root node for model.
    member this.UnderlyingModel = model

    interface ILanguageModel with

        member this.Name 
            with get () = model.Name
            and set v = model.Name <- v

        member this.HasMetamodel =
            failwith "Not implemented"

        member this.Metamodel =
            pool.WrapModel model.Metamodel

        member this.CreateNode name =
            (this :> ILanguageModel).InstantiateNode 
                name 
                ((pool.Wrap attributeMetamodelNode) :?> ILanguageNode) 
                Map.empty

        member this.CreateGeneralization source target =
            wrap <| model.CreateGeneralization (unwrap source) (unwrap target) :?> ILanguageGeneralization

        member this.CreateAssociation source target targetName =
            wrap <| model.CreateAssociation (unwrap source) (unwrap target) targetName :?> ILanguageAssociation

        member this.InstantiateNode name metatype attributeValues =
            let node = model.InstantiateNode name (unwrap metatype :?> IAttributeNode) Map.empty
            pool.Wrap node :?> ILanguageNode

        member this.InstantiateAssociation source target metatype attributeValues =
            let edge = 
                model.InstantiateAssociation 
                    (unwrap source) 
                    (unwrap target) 
                    (unwrap metatype :?> IAttributeAssociation)
                    Map.empty
            wrap edge :?> ILanguageAssociation

        member this.CreateEnumeration name elements =
            let enumerationNode = model.InstantiateNode name enumerationNode Map.empty
            elements
            |> Seq.map (fun e -> model.InstantiateNode e stringNode Map.empty)
            |> Seq.iter (fun n -> model.InstantiateAssociation enumerationNode n enumElementsAssociation Map.empty 
                                  |> ignore)

            enumerationNode |> pool.WrapEnumeration

        member this.Elements = model.Elements |> Seq.map wrap

        member this.Nodes = model.Nodes |> Seq.map wrap |> Seq.cast<ILanguageNode>

        member this.Edges = model.Edges |> Seq.map wrap |> Seq.cast<ILanguageEdge>

        member this.DeleteElement element =
            model.DeleteElement (unwrap element)

        member this.Node name =
            (this :> ILanguageModel).Nodes
            |> Seq.filter (fun n -> n.Name = name)
            |> Helpers.exactlyOneElement name

        member this.HasNode name =
            (this :> ILanguageModel).Nodes
            |> Seq.filter (fun n -> n.Name = name)
            |> Seq.isEmpty
            |> not

        member this.Association name =
            (this :> ILanguageModel).Edges
            |> Seq.choose (fun e -> if e :? ILanguageAssociation then Some (e :?> ILanguageAssociation) else None)
            |> Seq.filter (fun e -> e.TargetName = name)
            |> Helpers.exactlyOneElement name

        member this.HasAssociation name =
            (this :> ILanguageModel).Edges
            |> Seq.choose (fun e -> if e :? ILanguageAssociation then Some (e :?> ILanguageAssociation) else None)
            |> Seq.filter (fun e -> e.TargetName = name)
            |> Seq.isEmpty
            |> not

        member this.PrintContents () =
            printfn "Model: %s" model.Name
            ()
