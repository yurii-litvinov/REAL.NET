(* Copyright 2019 Yurii Litvinov
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

module Repo.CoreMetamodel.CoreSemanticsHelpers

open Repo.DataLayer
open System.Collections.Generic

/// Does a breadth-first search from a given element following given interesting edges until element matching
/// given predicate is found.
let bfs (element: IDataElement) 
        (isInterestingEdge: IDataEdge -> bool) 
        (isWhatWeSearch: IDataElement -> bool) =
    let queue = Queue<IDataElement>()
    queue.Enqueue element
    let visited = HashSet<IDataElement>()
    let rec doBfs () =
        if queue.Count = 0 then
            None
        else
            let currentElement = queue.Dequeue ()
            if isWhatWeSearch currentElement then
                Some currentElement
            else
                visited.Add currentElement |> ignore
                currentElement.OutgoingEdges
                |> Seq.filter isInterestingEdge
                |> Seq.map (fun e -> e.Target)
                |> Seq.choose id
                |> Seq.filter (not << visited.Contains)
                |> Seq.iter queue.Enqueue
                doBfs()

    doBfs ()

/// Returns true if given element is generalization edge.
let isGeneralization (e: IDataElement) = e :? IDataGeneralization

/// Returns true if given element is association edge.
let isAssociation (e: IDataElement) = e :? IDataAssociation
