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

namespace RepoExperimental.FacadeLayer

open RepoExperimental

type Edge() = 
    interface IEdge with
        member this.InstanceMetatype: Metatype = 
            raise (System.NotImplementedException())
        member this.Metatype: Metatype = 
            raise (System.NotImplementedException())
        member this.Attributes = raise (System.NotImplementedException())
        member this.From
            with get (): IElement = 
                raise (System.NotImplementedException())
            and set (v: IElement): unit = 
                raise (System.NotImplementedException())
        member this.IsAbstract = raise (System.NotImplementedException())
        member this.Shape = raise (System.NotImplementedException())
        member this.To
            with get (): IElement = 
                raise (System.NotImplementedException())
            and set (v: IElement): unit = 
                raise (System.NotImplementedException())