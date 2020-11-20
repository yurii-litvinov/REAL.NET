﻿(* Copyright 2019 REAL.NET group
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

namespace Repo.Visual

open Repo

/// Implements class - representation of edge on screen
type VisualEdgeInfo
    (
        routingPoints : VisualPoint seq
    ) =
    let mutable routingPoints = routingPoints

    new() = new VisualEdgeInfo([||])
    
    interface IVisualEdgeInfo with
        member this.RoutingPoints
            with get () = Array.ofSeq routingPoints             
            and set (v) = routingPoints <- v
            
        member this.Copy() =
            let points = (this :> IVisualEdgeInfo).RoutingPoints
            new VisualEdgeInfo(Array.copy points) :> IVisualEdgeInfo    