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

namespace RepoExperimental.InfrastructureSemanticLayer

open RepoExperimental
open RepoExperimental.DataLayer

/// Helper functions to work with Infrastructure Metamodel.
module InfrastructureMetamodel =
    /// Searches for an Infrastructure Metamodel in current repository.
    let private infrastructureMetamodel (repo: IRepo) =
        let models = 
            repo.Models 
            |> Seq.filter (fun m -> m.Name = "InfrastructureMetamodel")
        
        if Seq.isEmpty models then
            raise (MalformedCoreMetamodelException "Infrastructure Metamodel not found in a repository")
        elif Seq.length models <> 1 then
            raise (MalformedCoreMetamodelException "There is more than one Infrastructure Metamodel in a repository")
        else
            Seq.head models

    let private node (repo: IRepo) =
        let metamodel = infrastructureMetamodel repo
        CoreSemanticLayer.Model.findNode metamodel "Node"

    let private edge (repo: IRepo) =
        let metamodel = infrastructureMetamodel repo
        CoreSemanticLayer.Model.findNode metamodel "Edge"

    let isNode (repo: IRepo) element =
        CoreSemanticLayer.Element.isInstanceOf (node repo) element

    let isEdge (repo: IRepo) element =
        CoreSemanticLayer.Element.isInstanceOf (edge repo) element
