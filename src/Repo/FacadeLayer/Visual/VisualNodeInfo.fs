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
        link: string,
        ``type``: TypeOfVisual,
        position : (int * int) option
    ) =

    inherit VisualInfo(link, ``type``)

    let mutable position = position

    /// Instantiates instance of class with default values.
    new() = VisualNodeInfo("", TypeOfVisual.NoFile, None)

    interface IVisualNodeInfo with
        member this.Position
            with get (): (int * int) option = position             
            and set (v: (int * int) option): unit = position <- v
