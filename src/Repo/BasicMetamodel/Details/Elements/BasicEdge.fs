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

open Repo.BasicMetamodel

/// Implementation of Edge abstraction.
type BasicEdge(source: IBasicElement, target: IBasicElement, targetName: string) as this =
    inherit BasicElement()

    let mutable source = source

    do
        (source :?> BasicElement).RegisterOutgoingEdge this

    override this.ToString () =
        targetName

    interface IBasicEdge with
        member this.Source
            with get () = source
            and set v =
                (source :?> BasicElement).UnregisterOutgoingEdge this
                source <- v
                (source :?> BasicElement).RegisterOutgoingEdge this

        member val Target = target with get,set

        member val TargetName = targetName with get,set
