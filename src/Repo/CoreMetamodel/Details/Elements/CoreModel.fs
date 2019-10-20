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

namespace Repo.CoreMetamodel.Details.Elements

open Repo
open Repo.CoreMetamodel

/// Implementation of model.
type CoreModel(modelNode: BasicMetamodel.IBasicNode, pool: CorePool, repo: BasicMetamodel.IBasicRepository) =

    let unwrap (element: ICoreElement) = (element :?> CoreElement).UnderlyingElement

    let coreMetamodelNode = repo.Node Consts.node
    let coreMetamodelGeneralization = repo.Node Consts.generalization
    let coreMetamodelAssociation = repo.Node Consts.association
    let coreMetamodelElementsEdge = (repo.Node Consts.model).OutgoingEdge Consts.elementsEdge
    let coreMetamodelModelEdge = 
        (repo.Node Consts.element).OutgoingEdges 
        |> Seq.filter (fun e -> e.TargetName = Consts.modelEdge)
        |> Seq.filter (fun e -> e.Metatype :? BasicMetamodel.IBasicEdge)
        |> Helpers.exactlyOneElement "models"

    let (--/-->) source target = repo.CreateEdge source target Consts.instanceOfEdge |> ignore
    let (--->) source (target, targetName) =
        repo.CreateEdge (unwrap source) (unwrap target) targetName
    let (~+) name = repo.CreateNode name
    let (++) model element =
        let elementsEdge = repo.CreateEdge model element Consts.elementsEdge
        elementsEdge --/--> coreMetamodelElementsEdge
        let modelEdge = repo.CreateEdge element model Consts.modelEdge
        modelEdge --/--> coreMetamodelModelEdge

    /// Returns underlying BasicNode that is a root node for model.
    member this.UnderlyingModel = modelNode

    interface ICoreModel with

        member this.Name 
            with get () = modelNode.Name
            and set v = modelNode.Name <- v

        member this.Metamodel =
            let metamodel = (modelNode.OutgoingEdge Consts.metamodelEdge).Target
            pool.WrapModel metamodel

        member this.CreateNode name =
            (this :> ICoreModel).InstantiateNode name ((pool.Wrap coreMetamodelNode) :?> ICoreNode)

        member this.CreateGeneralization source target =
            let edge = source ---> (target, "")
            edge --/--> coreMetamodelGeneralization
            modelNode ++ edge
            pool.Wrap edge :?> ICoreGeneralization

        member this.CreateAssociation source target targetName =
            let edge = source ---> (target, targetName)
            edge --/--> coreMetamodelAssociation
            modelNode ++ edge
            pool.Wrap edge :?> ICoreAssociation

        member this.InstantiateNode name metatype =
            let node = +name
            node --/--> (unwrap metatype)
            modelNode ++ node
            pool.Wrap node :?> ICoreNode

        member this.InstantiateAssociation source target metatype =
            let targetName = ((unwrap metatype) :?> BasicMetamodel.IBasicEdge).TargetName
            let edge = source ---> (target, targetName)
            edge --/--> unwrap metatype
            modelNode ++ edge
            pool.Wrap edge :?> ICoreAssociation

        member this.Elements =
            let isElementsEdge (edge: Repo.BasicMetamodel.IBasicEdge) =
                let modelMetametatype = repo.Node Consts.metamodelModel
                let elementsMetametatype = 
                    modelMetametatype.OutgoingEdge Consts.elementsEdge :> BasicMetamodel.IBasicElement
                let modelMetatype = repo.Node Consts.model
                let elementsMetatype = 
                    modelMetatype.OutgoingEdge Consts.elementsEdge :> BasicMetamodel.IBasicElement
                (edge.Metatype = elementsMetametatype) || (edge.Metatype = elementsMetatype)

            modelNode.OutgoingEdges
            |> Seq.filter (fun e -> (e.Metatypes |> Seq.isEmpty |> not) && (isElementsEdge e))
            |> Seq.map (fun e -> e.Target)
            |> Seq.map pool.Wrap

        member this.Nodes =
            (this :> ICoreModel).Elements
            |> Seq.filter (fun e -> e :? ICoreNode)
            |> Seq.cast<ICoreNode>

        member this.Edges =
            (this :> ICoreModel).Elements
            |> Seq.filter (fun e -> e :? ICoreEdge)
            |> Seq.cast<ICoreEdge>

        member this.DeleteElement element =
            let edges = Seq.append element.OutgoingEdges element.IncomingEdges |> Seq.toList
            edges |> Seq.iter (this :> ICoreModel).DeleteElement
            pool.UnregisterElement (element :?> CoreElement).UnderlyingElement
            repo.DeleteElement (element :?> CoreElement).UnderlyingElement

        member this.Node name =
            (this :> ICoreModel).Nodes
            |> Seq.filter (fun n -> n.Name = name)
            |> Helpers.exactlyOneElement name

        member this.HasNode name =
            (this :> ICoreModel).Nodes
            |> Seq.filter (fun n -> n.Name = name)
            |> Seq.isEmpty
            |> not

        member this.Association name =
            (this :> ICoreModel).Edges
            |> Seq.choose (fun e -> if e :? ICoreAssociation then Some (e :?> ICoreAssociation) else None)
            |> Seq.filter (fun e -> e.TargetName = name)
            |> Helpers.exactlyOneElement name

        member this.HasAssociation name =
            (this :> ICoreModel).Edges
            |> Seq.choose (fun e -> if e :? ICoreAssociation then Some (e :?> ICoreAssociation) else None)
            |> Seq.filter (fun e -> e.TargetName = name)
            |> Seq.isEmpty
            |> not

        member this.PrintContents () =
            printfn "Model: %s" modelNode.Name
            ()
