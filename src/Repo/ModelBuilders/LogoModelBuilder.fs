(* Copyright 2017-2019 REAL.NET group
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

namespace Repo.Metametamodels

open Repo.DataLayer
open Repo.CoreSemanticLayer
open Repo.InfrastructureSemanticLayer

/// Initializes repository with test model conforming to Logo Metamodel, actual program that can be written by end-user.
type LogoModelBuilder() =
   interface IModelBuilder with
       member this.Build(repo: IRepo): unit =
           let infrastructure = InfrastructureSemantic(repo)
           let metamodel = Repo.findModel repo "LogoMetamodel"
           let infrastructureMetamodel = infrastructure.Metamodel.Model
           0 |> ignore