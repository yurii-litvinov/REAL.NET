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

namespace Repo.Visual

open Repo

/// Implements class - representation of edge on screen
type VisualEdgeInfo
    (
        link: string,
        ``type``: TypeOfVisual,
        routingPoints : (int * int) list
    ) =

    inherit VisualInfo(link, ``type``)

    let mutable routingPoints = routingPoints

    /// Instantiates instance of class with default values.
    new() = VisualEdgeInfo("", TypeOfVisual.NoFile, [])

    interface IVisualEdgeInfo with
        member this.RoutingPoints
            with get () = routingPoints             
            and set (v) = routingPoints <- v       