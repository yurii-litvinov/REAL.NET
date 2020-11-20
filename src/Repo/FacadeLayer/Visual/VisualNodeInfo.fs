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

/// Implements class - representation of node on screen
type VisualNodeInfo
    (
        position : VisualPoint
    ) =
    let mutable position = position
    
    new() = new VisualNodeInfo(VisualPoint.Default)

    /// Instantiates instance of class with default values.
    interface IVisualNodeInfo with
        member this.Position
            with get () = position             
            and set v: unit = position <- v

        member this.Copy() = new VisualNodeInfo(position) :> IVisualNodeInfo