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

namespace Repo.CoreMetamodel.Details.Elements

open Repo.CoreMetamodel
open Repo.BasicMetamodel

open System.Collections.Generic

/// Cache for wrapper objects. Allows to request wrapper object, store wrapped object, remove wrapped object.
/// Does not provide any consistency checks.
type CorePool(factory: ICoreFactory) =
    let elementsPool = Dictionary<IBasicElement, ICoreElement>() :> IDictionary<_, _>
    let modelsPool = Dictionary<IBasicElement, ICoreModel>() :> IDictionary<_, _>
    
    member this.Wrap (element: IBasicElement): ICoreElement =
        if elementsPool.ContainsKey element then
            elementsPool.[element]
        else 
            let wrapper = factory.CreateElement element this
            elementsPool.Add(element, wrapper)
            wrapper

    member this.UnregisterElement (element: IBasicElement): unit =
        if not <| elementsPool.Remove element then 
            failwith "Removing non-existent element"

    member this.WrapModel (model: IBasicElement): ICoreModel =
        if modelsPool.ContainsKey model then
            modelsPool.[model]
        else 
            let wrapper = factory.CreateModel (model :?> IBasicNode) this
            modelsPool.Add(model, wrapper)
            wrapper

and ICoreFactory =
    abstract CreateElement: element: IBasicElement -> pool: CorePool -> ICoreElement
    abstract CreateModel: model: IBasicNode -> pool: CorePool -> ICoreModel
