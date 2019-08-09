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

namespace Repo.DataLayer

/// Implementation of a node in model.
type DataNode private 
        (
        name: string, 
        ontologicalType: IDataElement option, 
        linguisticType: IDataElement option, 
        model: IDataModel
        ) =
    inherit DataElement(ontologicalType, linguisticType, model)

    let mutable name = name
    
    new (name: string, model: IDataModel) = DataNode(name, None, None, model)
    new (name: string, ontologicalType: IDataElement, linguisticType: IDataElement, model: IDataModel) = 
        DataNode(name, Some ontologicalType, Some linguisticType, model)

    override this.ToString () = model.Name + "." + name

    interface IDataNode with
        member __.Name
            with get(): string = name
            and set v = name <- v
