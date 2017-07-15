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

open System.Collections.Generic

open RepoExperimental

///Model repository. Holds all already created wrappers for data models, creates them as needed.
type ModelRepository(repo: DataLayer.IRepo) =
    let models = Dictionary<DataLayer.IModel, Model>()
    member this.GetModel (data: DataLayer.IModel) =
        if models.ContainsKey data then
            models.[data] :> IModel
        else
            let newModel = Model(repo, data, this)
            models.Add (data, newModel)
            newModel :> IModel
    
    member this.DeleteModel (model: IModel) = 
        let unwrappedModel = (model :?> Model).UnderlyingModel
        models.Remove unwrappedModel |> ignore

and Model(repo: DataLayer.IRepo, data: DataLayer.IModel, repository: ModelRepository) = 
    let attributeRepository = AttributeRepository(repo)
    let elementRepository = ElementRepository(repo, data, attributeRepository) :> IElementRepository

    member this.UnderlyingModel = data

    interface IModel with
        member this.CreateElement ``type`` = raise (System.NotImplementedException())
        member this.DeleteElement element = raise (System.NotImplementedException())
        
        member this.Nodes = 
            data.Nodes 
            |> Seq.filter (InfrastructureSemanticLayer.InfrastructureMetamodel.isNode repo)
            |> Seq.map elementRepository.GetElement
            |> Seq.cast<INode>

        member this.Edges = 
            data.Edges 
            |> Seq.filter (InfrastructureSemanticLayer.InfrastructureMetamodel.isEdge repo)
            |> Seq.map elementRepository.GetElement
            |> Seq.cast<IEdge>

        member this.Metamodel = 
            repository.GetModel data.Metamodel

        member this.Name
            with get () = data.Name
            and set v = data.Name <- v
