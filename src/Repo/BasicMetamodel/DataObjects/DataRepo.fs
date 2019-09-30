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

    interface IDataRepository with
        member this.CreateModel(name: string): IDataModel =
            let model = DataModel(name) :> IDataModel
            models <- model :: models
            model

        member this.CreateModel
                (
                name: string, 
                ontologicalMetamodel: IDataModel, 
                linguisticMetamodel: IDataModel
                ) : IDataModel =
            let model = DataModel(name, ontologicalMetamodel, linguisticMetamodel) :> IDataModel
            models <- model :: models
            model

        member this.DeleteModel(model: IDataModel): unit =
            if models |> List.exists (fun m -> m.OntologicalMetamodel = model &&  m <> model) then
                raise (Repo.DeletingUsedModel(model.Name))
            models <- models |> List.filter (fun m -> not (m.Equals(model)))

        member this.Models: seq<IDataModel> =
            Seq.ofList models |> Seq.cast

        member this.Clear () =
            models <- []
        
        member this.Model (name: string): IDataModel =
            Repo.Helpers.getExactlyOne models
                    (fun m -> m.Name = name)
                    (fun () -> Repo.ModelNotFoundException name)
                    (fun () -> Repo.MultipleModelsException name)
