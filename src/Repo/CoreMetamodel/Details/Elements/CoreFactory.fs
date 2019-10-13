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
open Repo.BasicMetamodel

/// Implementation of wrapper factory.
type CoreFactory(repo: IBasicRepository) =
    let metaGeneralization () = 
        if repo.HasNode CoreMetamodel.Consts.generalization then 
            Some (repo.Node CoreMetamodel.Consts.generalization :> IBasicElement)
        else 
            None

    let metametaGeneralization () = 
        if repo.HasNode CoreMetamodel.Consts.metamodelGeneralization then 
            Some (repo.Node CoreMetamodel.Consts.metamodelGeneralization :> IBasicElement)
        else 
            None

    interface ICoreFactory with
        member this.CreateElement element pool =
            match element with
            | :? IBasicNode as n -> CoreNode(n, pool, repo) :> ICoreElement
            | :? IBasicEdge as e ->
                let metaGeneralization = metaGeneralization ()
                let metametaGeneralization = metametaGeneralization ()
                if metaGeneralization.IsSome 
                    && metametaGeneralization.IsSome
                    && e.Metatypes |> Seq.isEmpty |> not 
                    && (e.Metatype = metaGeneralization.Value || e.Metatype = metametaGeneralization.Value)
                then
                    CoreGeneralization(e, pool, repo) :> ICoreElement
                elif e.TargetName = BasicMetamodel.Consts.instanceOf then
                    CoreInstanceOf(e, pool, repo) :> ICoreElement
                else
                    CoreAssociation(e, pool, repo) :> ICoreElement
            | _ -> failwith "Unknown subtype"

        member this.CreateModel model pool =
            CoreModel(model, pool, repo) :> ICoreModel
