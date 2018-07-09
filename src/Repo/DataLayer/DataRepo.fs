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

namespace Repo.DataLayer

/// Implementation of repository that contains models in a list.
type DataRepo() =
    let mutable models = []

    interface IRepo with
        member this.CreateModel(name: string): IModel =
            let model = DataModel(name) :> IModel
            models <- model :: models
            model

        member this.CreateModel(name: string, metamodel: IModel): IModel =
            let model = DataModel(name, metamodel) :> IModel
            models <- model :: models
            model

        member this.DeleteModel(model: IModel): unit =
            if models |> List.exists (fun m -> m.Metamodel = model &&  m <> model) then
                raise (Repo.DeletingUsedModel(model.Name))
            models <- models |> List.filter (fun m -> not (m.Equals(model)))

        member this.Models: seq<IModel> =
            Seq.ofList models |> Seq.cast

        member this.Clear () =
            models <- []
