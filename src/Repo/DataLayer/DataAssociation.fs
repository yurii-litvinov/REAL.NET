﻿(* Copyright 2017 Yurii Litvinov
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

/// Implementation of Association edge.
type DataAssociation
    (
        ``class``: IDataElement
        , source: IDataElement option
        , target: IDataElement option
        , nameTarget: string
        , model: IDataModel
        ) =
    inherit DataEdge(Some ``class``, source, target, model)

    interface IAssociation with
        member val TargetName: string = nameTarget with get, set
