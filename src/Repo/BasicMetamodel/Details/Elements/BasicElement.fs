(* Copyright 2019 REAL.NET group
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

namespace Repo.BasicMetamodel.Details.Elements

open Repo
open Repo.BasicMetamodel

/// Implementation of Element.
[<AbstractClass>]
type BasicElement() =
    let mutable outgoingEdges = []

    member this.RegisterOutgoingEdge edge =
        outgoingEdges <- edge :: outgoingEdges

    member this.UnregisterOutgoingEdge edge =
        outgoingEdges <- outgoingEdges |> List.except [edge]
    
    interface IBasicElement with
        member this.OutgoingEdges =
            Seq.ofList outgoingEdges

        member this.OutgoingEdge name =
            outgoingEdges 
            |> Seq.filter (fun e -> e.TargetName = name)
            |> Helpers.exactlyOneElement name

        member this.HasExactlyOneOutgoingEdge name =
            outgoingEdges 
            |> Seq.filter (fun e -> e.TargetName = name)
            |> Seq.length
            |> (=) 1

        member this.Metatypes =
            outgoingEdges
            |> Seq.filter (fun e -> e.TargetName = "instanceOf")
            |> Seq.filter (fun e -> e.Metatypes |> Seq.isEmpty)
            |> Seq.map (fun e -> e.Target)

        member this.Metatype =
            (this :> IBasicElement).Metatypes
            |> Helpers.exactlyOneElement "instanceOf"
