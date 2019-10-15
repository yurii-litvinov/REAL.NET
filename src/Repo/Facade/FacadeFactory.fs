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

namespace Repo.Facade

open Repo
open Repo.InfrastructureMetamodel

/// Implementation of wrapper factory.
type FacadeFactory(repo: IInfrastructureRepository) =
    interface IFacadeFactory with
        member this.CreateElement element pool =
            match element with
            | :? IInfrastructureNode as n -> Node(n, pool, repo) :> IElement
            //| :? IInfrastructureGeneralization as g -> Generalization(g, pool, repo) :> IElement
            //| :? IInfrastructureInstanceOf as i -> InstanceOf(i, pool, repo) :> IElement
            //| :? IInfrastructureAssociation as a -> Association(a, pool, repo) :> IElement
            | _ -> failwith "Unknown subtype"

        member this.CreateModel model pool =
            Model(model, pool, repo) :> IModel
