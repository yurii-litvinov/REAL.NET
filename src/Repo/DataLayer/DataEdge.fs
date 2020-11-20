﻿ (* Copyright 2017 Yurii Litvinov
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

/// Implementation of Edge abstraction.
[<AbstractClass>]
type DataEdge (``class``: IDataElement option, source: IDataElement option, target: IDataElement option, model: IDataModel) =
    inherit DataElement(``class``, model)

    let mutable source = source
    let mutable target = target

    interface IDataEdge with
        member this.Source
            with get () = source
            and set v =
                if source.IsSome then
                    source.Value.DeleteOutgoingEdge this
                source <- v
                if source.IsSome then
                    source.Value.AddOutgoingEdge this

        member this.Target
            with get () = target
            and set v =
                if source.IsSome then
                    source.Value.DeleteIncomingEdge this
                target <- v
                if target.IsSome then
                    target.Value.AddIncomingEdge this
