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

type Repo(data : RepoExperimental.DataLayer.IRepo) =
    let modelRepository = ModelRepository()

    interface RepoExperimental.IRepo with
        member this.Models
            with get () =
                data.Models |> Seq.map modelRepository.GetModel |> Seq.map (fun m -> m :> IModel)

        member this.CreateModel name metamodel =
            raise (new System.NotImplementedException())
        
        member this.DeleteModel model =
            raise (new System.NotImplementedException())
