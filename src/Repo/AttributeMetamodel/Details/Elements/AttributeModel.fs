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

/// Implementation of model.
type AttributeModel(model: CoreMetamodel.ICoreModel, pool: AttributePool, repo: CoreMetamodel.ICoreRepository) =

    let unwrap (element: IAttributeElement) = (element :?> AttributeElement).UnderlyingElement
    let wrap element = pool.Wrap element

    let attributeMetamodel = repo.Model Consts.attributeMetamodel

    let attributeMetamodelNode = attributeMetamodel.Node Consts.node
    let slotMetatype = attributeMetamodel.Node Consts.slot
    let slotsAssociationMetatype = attributeMetamodel.Association Consts.slotsEdge
    let valueAssociationMetatype = attributeMetamodel.Association Consts.valueEdge
    let attributeAssociationMetatype = attributeMetamodel.Association Consts.attributeEdge

    let (--->) source (target, metatype) =
        model.InstantiateAssociation (unwrap source) (unwrap target) metatype |> ignore

   /// Returns underlying BasicNode that is a root node for model.
    member this.UnderlyingModel = model

    interface IAttributeModel with

        member this.Name 
            with get () = model.Name
            and set v = model.Name <- v

        member this.HasMetamodel =
            failwith "Not implemented"

        member this.Metamodel =
            pool.WrapModel model.Metamodel

        member this.CreateNode name =
            (this :> IAttributeModel).InstantiateNode 
                name 
                ((pool.Wrap attributeMetamodelNode) :?> IAttributeNode) 
                Map.empty

        member this.CreateGeneralization source target =
            wrap <| model.CreateGeneralization (unwrap source) (unwrap target) :?> IAttributeGeneralization

        member this.CreateAssociation source target targetName =
            wrap <| model.CreateAssociation (unwrap source) (unwrap target) targetName :?> IAttributeAssociation

        member this.InstantiateNode name metatype attributeValues =
            let addSlot (node: IAttributeElement) (attribute: IAttributeAttribute) =
                let value = attributeValues.[attribute.Name]
                let slotNode = model.InstantiateNode name slotMetatype
                node ---> (wrap slotNode, slotsAssociationMetatype)
                wrap slotNode ---> (value, valueAssociationMetatype)
                let unwrappedAttribute = (attribute :?> AttributeAttribute).UnderlyingElement
                model.InstantiateAssociation slotNode unwrappedAttribute attributeAssociationMetatype |> ignore

            let node = model.InstantiateNode name (unwrap metatype :?> CoreMetamodel.ICoreNode)

            metatype.Attributes
            |> Seq.iter (addSlot (wrap node))

            pool.Wrap node :?> IAttributeNode

        member this.InstantiateAssociation source target metatype attributeValues =
            let edge = 
                model.InstantiateAssociation 
                    (unwrap source) 
                    (unwrap target) 
                    (unwrap metatype :?> CoreMetamodel.ICoreAssociation)
            wrap edge :?> IAttributeAssociation

        member this.Elements = model.Elements |> Seq.map wrap

        member this.Nodes = model.Nodes |> Seq.map wrap |> Seq.cast<IAttributeNode>

        member this.Edges = model.Edges |> Seq.map wrap |> Seq.cast<IAttributeEdge>

        member this.DeleteElement element =
            model.DeleteElement (unwrap element)

        member this.Node name =
            (this :> IAttributeModel).Nodes
            |> Seq.filter (fun n -> n.Name = name)
            |> Helpers.exactlyOneElement name

        member this.HasNode name =
            (this :> IAttributeModel).Nodes
            |> Seq.filter (fun n -> n.Name = name)
            |> Seq.isEmpty
            |> not

        member this.Association name =
            (this :> IAttributeModel).Edges
            |> Seq.choose (fun e -> if e :? IAttributeAssociation then Some (e :?> IAttributeAssociation) else None)
            |> Seq.filter (fun e -> e.TargetName = name)
            |> Helpers.exactlyOneElement name

        member this.HasAssociation name =
            (this :> IAttributeModel).Edges
            |> Seq.choose (fun e -> if e :? IAttributeAssociation then Some (e :?> IAttributeAssociation) else None)
            |> Seq.filter (fun e -> e.TargetName = name)
            |> Seq.isEmpty
            |> not

        member this.PrintContents () =
            printfn "Model: %s" model.Name
            ()
