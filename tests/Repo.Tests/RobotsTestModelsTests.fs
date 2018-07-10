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

module RobotsTestModelsTests

open NUnit.Framework
open FsUnit

open Repo

let init () =
    RepoFactory.Create ()

[<Test>]
let ``Generalizations shall be listed as edges in metamodel`` () =
    let repo = init ()
    let rawRepo = (repo :?> FacadeLayer.Repo).UnderlyingRepo
    let infrastructure = InfrastructureSemanticLayer.InfrastructureSemantic rawRepo
    let infrastructureModel = repo.Model "InfrastructureMetamodel"
    let generalization = infrastructureModel.Nodes |> Seq.find (fun n -> n.Name = "Generalization")
    let model = repo.Model "RobotsMetamodel"

    let rawEdges = model.Edges |> Seq.map (fun e -> (e :?> FacadeLayer.Element).UnderlyingElement :?> DataLayer.IEdge)
    let rawGeneralization = (generalization :?> FacadeLayer.Element).UnderlyingElement
    let rawGeneralizations = rawEdges |> Seq.filter (fun e -> e.Class = rawGeneralization)
    let someRawGeneralization = rawGeneralizations |> Seq.head

    someRawGeneralization.Class |> should sameAs rawGeneralization

    infrastructure.Metamodel.IsGeneralization someRawGeneralization |> should be True
    infrastructure.Metamodel.IsEdge someRawGeneralization |> should be True

    let someWrappedGeneralization =
        model.Edges
        |> Seq.find
            (fun e -> (e :?> FacadeLayer.Edge).UnderlyingElement = (someRawGeneralization :> DataLayer.IElement))

    someWrappedGeneralization.Class |> should sameAs generalization

    model.Edges |> Seq.filter (fun e -> e.Class = (generalization :> IElement)) |> should not' (be Empty)
