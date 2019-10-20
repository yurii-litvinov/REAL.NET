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

namespace Repo.AttributeMetamodel.Details.Elements

open Repo.AttributeMetamodel
open Repo.CoreMetamodel

open System.Collections.Generic

/// Cache for wrapper objects. Allows to request wrapper object, store wrapped object, remove wrapped object.
/// Does not provide any consistency checks.
type AttributePool(factory: IAttributeFactory) =
    let elementsPool = Dictionary<ICoreElement, IAttributeElement>() :> IDictionary<_, _>
    let modelsPool = Dictionary<ICoreModel, IAttributeModel>() :> IDictionary<_, _>
    let attributesPool = Dictionary<ICoreElement, IAttributeAttribute>() :> IDictionary<_, _>
    let slotsPool = Dictionary<ICoreElement, IAttributeSlot>() :> IDictionary<_, _>

    let wrap (pool: IDictionary<'a, 'b>) (factory: 'a -> 'b) (element: 'a): 'b =
        if pool.ContainsKey element then
            pool.[element]
        else 
            let wrapper = factory element
            pool.Add(element, wrapper)
            wrapper

    let unregister (pool: IDictionary<'a, 'b>) (element: 'a) =
        if not <| pool.Remove element then 
            failwith "Removing non-existent element"

    /// Wraps given CoreElement to AttributeElement. Creates new wrapper if needed, otherwise returns cached copy.
    member this.Wrap (element: ICoreElement): IAttributeElement =
        wrap elementsPool (fun e -> factory.CreateElement e this) element

    /// Removes element from cache.
    member this.UnregisterElement (element: ICoreElement): unit =
        unregister elementsPool element

    /// Wraps given CoreElement to AttributeAttribute. Creates new wrapper if needed, otherwise returns cached copy.
    member this.WrapAttribute (element: ICoreElement): IAttributeAttribute =
        wrap attributesPool (fun e -> factory.CreateAttribute e this) element

    /// Removes attribute from cache.
    member this.UnregisterAttribute (element: ICoreElement): unit =
        unregister attributesPool element

    /// Wraps given CoreElement to AttributeSlot. Creates new wrapper if needed, otherwise returns cached copy.
    member this.WrapSlot (element: ICoreElement): IAttributeSlot =
        wrap slotsPool (fun e -> factory.CreateSlot e this) element

    /// Removes slot from cache.
    member this.UnregisterSlot (element: ICoreElement): unit =
        unregister slotsPool element

    /// Wraps given node to CoreModel. Creates new wrapper if needed, otherwise returns cached copy.
    member this.WrapModel (model: ICoreModel): IAttributeModel =
        wrap modelsPool (fun e -> factory.CreateModel e this) model

    /// Removes model from cache.
    member this.UnregisterModel (model: ICoreModel): unit =
        unregister modelsPool model

    /// Clears cached values, invalidating all references to Core elements.
    member this.Clear () =
        elementsPool.Clear ()
        modelsPool.Clear ()
        attributesPool.Clear ()
        slotsPool.Clear ()

/// Abstract factory that creates wrapper objects.
and IAttributeFactory =
    /// Creates AttributeElement wrapper by given CoreElement.
    abstract CreateElement: element: ICoreElement -> pool: AttributePool -> IAttributeElement

    /// Creates AttributeModel wrapper by given CoreModel.
    abstract CreateModel: model: ICoreModel -> pool: AttributePool -> IAttributeModel

    /// Creates AttributeAttribute wrapper by given CoreElement.
    abstract CreateAttribute: element: ICoreElement -> pool: AttributePool -> IAttributeAttribute

    /// Creates AttributeSlot wrapper by given CoreElement.
    abstract CreateSlot: element: ICoreElement -> pool: AttributePool -> IAttributeSlot
