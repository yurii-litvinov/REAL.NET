//(* Copyright 2019 Yurii Litvinov
//*
//* Licensed under the Apache License, Version 2.0 (the "License");
//* you may not use this file except in compliance with the License.
//* You may obtain a copy of the License at
//*
//*     http://www.apache.org/licenses/LICENSE-2.0
//*
//* Unless required by applicable law or agreed to in writing, software
//* distributed under the License is distributed on an "AS IS" BASIS,
//* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//* See the License for the specific language governing permissions and
//* limitations under the License. *)

///// Module that contains helper functions that are useful in Core Metamodel semantics and in other semantic definitions
///// that are based on Core Metamodel.
//module Repo.CoreMetamodel.Details.CoreSemanticsHelpers

//open Repo.DataLayer
//open System.Collections.Generic

///// Does a breadth-first search from a given element following given interesting edges until element matching
///// given predicate is found.
//let bfs (element: IDataElement) 
//        (isInterestingEdge: IDataEdge -> bool) 
//        (isWhatWeSearch: IDataElement -> bool) =
//    let queue = Queue<IDataElement>()
//    queue.Enqueue element
//    let visited = HashSet<IDataElement>()
//    let rec doBfs () =
//        if queue.Count = 0 then
//            None
//        else
//            let currentElement = queue.Dequeue ()
//            if isWhatWeSearch currentElement then
//                Some currentElement
//            else
//                visited.Add currentElement |> ignore
//                currentElement.OutgoingEdges
//                |> Seq.filter isInterestingEdge
//                |> Seq.map (fun e -> e.Target)
//                |> Seq.choose id
//                |> Seq.filter (not << visited.Contains)
//                |> Seq.iter queue.Enqueue
//                doBfs()

//    doBfs ()

///// Returns true if given element is generalization edge.
//let isGeneralization (e: IDataElement) = e :? IDataGeneralization

///// Returns true if given element is association edge.
//let isAssociation (e: IDataElement) = e :? IDataAssociation

///// Helper function that creates a copy of a given edge in a given model (identifying source and target by name
///// and assuming they already present in a model).
//let reinstantiateEdge (edge: IDataEdge) (model: IDataModel) (linguisticMetamodel: IDataModel) =
//    let sourceName = (edge.Source.Value :?> IDataNode).Name
//    let targetName = (edge.Target.Value :?> IDataNode).Name
//    let source = model.Node sourceName
//    let target = model.Node targetName

//    let generalization = linguisticMetamodel.Node "Generalization"
//    let association = linguisticMetamodel.Node "Association"

//    match edge with 
//    | :? IDataGeneralization ->
//        model.CreateGeneralization(generalization, generalization, source, target) |> ignore
//    | :? IDataAssociation as a ->
//        model.CreateAssociation(association, association, source, target, a.TargetName) |> ignore
//    | _ -> failwith "Unknown edge type"
