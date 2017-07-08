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

namespace RepoExperimental.FacadeLayer

open RepoExperimental

type ModelRepository() =
    let models = System.Collections.Generic.Dictionary<RepoExperimental.DataLayer.IModel, Model>()
    member this.GetModel (data: RepoExperimental.DataLayer.IModel) =
        if models.ContainsKey data then
            models.[data]
        else
            let newModel = Model(data, this)
            models.Add (data, newModel)
            newModel
        
and Model(data: RepoExperimental.DataLayer.IModel, repository: ModelRepository) = 
    interface IModel with
        member this.CreateElement ``type`` = raise (System.NotImplementedException())
        member this.DeleteElement element = raise (System.NotImplementedException())
        
        member this.Nodes = raise (System.NotImplementedException())
        member this.Edges = raise (System.NotImplementedException())

        member this.Metamodel = 
            repository.GetModel data.Metamodel :> IModel

        member this.Name
            with get () = data.Name
            and set v = data.Name <- v
