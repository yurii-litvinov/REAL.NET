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
//open Repo.Serializer

/// Wrapper around repository from data layer. Maintains a repository of models and creates new models if needed.
type Repo(pool: FacadePool, repo: InfrastructureMetamodel.IInfrastructureRepository) =
//    let infrastructureMetamodel = repo.Model InfrastructureMetamodel.Consts.infrastructureMetamodel
//    let infrastructure = InfrastructureMetamodel.Semantics.InstantiationSemantics(infrastructureMetamodel)

//    let attributeRepository = AttributeRepository(repo)
//    let elementRepository = ElementRepository(infrastructure, attributeRepository, repo)
//    let modelRepository = ModelRepository(infrastructure, elementRepository)

    let wrap (model: IInfrastructureModel) = pool.WrapModel model
    let unwrap (model: IModel) = (model :?> Model).UnderlyingModel

    /// Returns wrapped repository from a data layer.
    member this.UnderlyingRepo = repo

    interface IRepo with
        member this.Models
            with get () =
                repo.Models |> Seq.map wrap

        member this.Model name =
            (this :> IRepo).Models 
            |> Seq.filter (fun m -> m.Name = name)
            |> Helpers.exactlyOneModel name

        member this.CreateModel (name: string, metamodel: IModel): IModel =
            let underlyingModel = repo.InstantiateModel name (unwrap metamodel)
            wrap underlyingModel

        member this.CreateModel (name, metamodelName) =
            let metamodel = (this :> IRepo).Model metamodelName
            (this :> IRepo).CreateModel (name, metamodel)

        member this.DeleteModel model =
            repo.DeleteModel (unwrap model)
            pool.UnregisterModel (unwrap model)

        member this.Save fileName =
            ()
            //Serializer.save fileName repo
