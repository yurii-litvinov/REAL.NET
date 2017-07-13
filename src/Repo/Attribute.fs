(* Copyright 2017 Yurii Litvinov
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

/// Repository for attribute wrappers. Contains already created wrappers and creates new wrappers if needed.
(*
type AttributeRepository() =
    let attributes = System.Collections.Generic.Dictionary<>()
    member this.GetAttribute () =
        if attributes.ContainsKey data then
            attributes.[data]
        else
            let newAttribute = Attribute(data, this)
            attributes.Add (data, newModel)
            newAttribute
    
    member this.DeleteAttribute () = 
        ()
*)

/// Implements attribute functionality
type Attribute() = 
    interface IAttribute with
        member this.Kind = raise (System.NotImplementedException())
        member this.Name = raise (System.NotImplementedException())
        member this.ReferenceValue
            with get (): IElement = 
                raise (System.NotImplementedException())
            and set (v: IElement): unit = 
                raise (System.NotImplementedException())
        member this.StringValue
            with get (): string = 
                raise (System.NotImplementedException())
            and set (v: string): unit = 
                raise (System.NotImplementedException())
        member this.Type = raise (System.NotImplementedException())