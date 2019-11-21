(* Copyright 2017-2018 REAL.NET group
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

/// Implementation of Element.
[<AbstractClass>]
type DataElement(``class``: IElement option, model: IModel) =
    let mutable outgoingEdges = []
    let mutable incomingEdges = []
    let mutable isMarkedDeleted = false

    interface IElement with
        member this.IsMarkedDeleted
            with get () = isMarkedDeleted
            and set (v) = isMarkedDeleted <- v 

        member this.Class: IElement =
            match ``class`` with
            | Some v -> v
            | None -> this :> IElement

        member this.OutgoingEdges =
            Seq.ofList outgoingEdges

        member this.IncomingEdges =
            Seq.ofList incomingEdges

        member this.AddOutgoingEdge edge = outgoingEdges <- edge :: outgoingEdges
        member this.AddIncomingEdge edge = incomingEdges <- edge :: incomingEdges

        member this.DeleteOutgoingEdge edge = outgoingEdges <- List.except [edge] outgoingEdges
        member this.DeleteIncomingEdge edge = incomingEdges <- List.except [edge] incomingEdges

        member this.Model = model

