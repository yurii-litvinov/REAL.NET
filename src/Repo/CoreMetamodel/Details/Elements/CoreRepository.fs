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

/// Implementation of core repository as a wrapper around basic repository.
type CoreRepository(pool: CorePool, repo: BasicMetamodel.IBasicRepository) =

    // Recalculated because repo is created empty and then filled with models.
    let modelMetametatype () = repo.Node Consts.metamodelModel :> BasicMetamodel.IBasicElement
    let modelMetatype () = repo.Node Consts.model :> BasicMetamodel.IBasicElement
    let metamodelEdge () = (modelMetatype ()).OutgoingEdge Consts.metamodelEdge
    let coreMetamodel () = repo.Node Consts.coreMetamodel

    let unwrap (model: ICoreModel) = (model :?> CoreModel).UnderlyingModel

    let (--/-->) source target = repo.CreateEdge source target Consts.instanceOfEdge |> ignore

    /// Returns underlying BasicRepository object.
    member this.UnderlyingRepo = repo

    interface ICoreRepository with

        member this.Models =
            let models = 
                repo.Nodes 
                |> Seq.filter (fun n -> n.Metatype = (modelMetametatype ()) || n.Metatype = (modelMetatype ()))
            
            models |> Seq.map pool.WrapModel

        member this.Model name =
            (this :> ICoreRepository).Models
            |> Seq.filter (fun m -> m.Name = name)
            |> Helpers.exactlyOneModel name

        member this.CoreMetamodel =
            (this :> ICoreRepository).Model Consts.coreMetamodel

        member this.InstantiateCoreMetamodel name =
            (this :> ICoreRepository).InstantiateModel name (pool.WrapModel (coreMetamodel ()))

        member this.InstantiateModel name metamodel =
            let model = repo.CreateNode name
            model --/--> modelMetatype ()
            let metamodelEdgeInstance = repo.CreateEdge model (unwrap metamodel) "metamodel"
            metamodelEdgeInstance --/--> metamodelEdge ()
            pool.WrapModel model

        member this.DeleteModel model =
            (this :> ICoreRepository).Models
            |> Seq.iter (fun m -> if m.Metamodel = model then raise (DeletingUsedModel model.Name))

            if model = pool.WrapModel (coreMetamodel ()) then
                raise (DeletingUsedModel model.Name)

            let modelElements = model.Elements |> Seq.toList
            modelElements
            |> Seq.iter (fun e -> if e.IsContainedInSomeModel then model.DeleteElement e)

            modelElements |> Seq.iter (fun e -> pool.UnregisterElement (e :?> CoreElement).UnderlyingElement)

            repo.DeleteElement (unwrap model)
            pool.UnregisterModel (unwrap model)

        member this.Clear () =
            pool.Clear ()
            repo.Clear ()
            ()
