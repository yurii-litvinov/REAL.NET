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

namespace Repo.LanguageMetamodel.Details.Elements

open Repo.LanguageMetamodel
open Repo.AttributeMetamodel

open System.Collections.Generic

/// Cache for wrapper objects. Allows to request wrapper object, store wrapped object, remove wrapped object.
/// Does not provide any consistency checks.
type LanguagePool(factory: ILanguageFactory) =
    let elementsPool = Dictionary<IAttributeElement, ILanguageElement>() :> IDictionary<_, _>
    let modelsPool = Dictionary<IAttributeModel, ILanguageModel>() :> IDictionary<_, _>
    let attributesPool = Dictionary<IAttributeAttribute, ILanguageAttribute>() :> IDictionary<_, _>
    let slotsPool = Dictionary<IAttributeSlot, ILanguageSlot>() :> IDictionary<_, _>
    let enumsPool = Dictionary<IAttributeElement, ILanguageEnumeration>() :> IDictionary<_, _>

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
    member this.Wrap (element: IAttributeElement): ILanguageElement =
        wrap elementsPool (fun e -> factory.CreateElement e this) element

    /// Removes element from cache.
    member this.UnregisterElement (element: IAttributeElement): unit =
        unregister elementsPool element

    /// Wraps given CoreElement to AttributeAttribute. Creates new wrapper if needed, otherwise returns cached copy.
    member this.WrapAttribute (attribute: IAttributeAttribute): ILanguageAttribute =
        wrap attributesPool (fun e -> factory.CreateAttribute e this) attribute

    /// Removes attribute from cache.
    member this.UnregisterAttribute (element: IAttributeAttribute): unit =
        unregister attributesPool element

    /// Wraps given CoreElement to AttributeSlot. Creates new wrapper if needed, otherwise returns cached copy.
    member this.WrapSlot (slot: IAttributeSlot): ILanguageSlot =
        wrap slotsPool (fun e -> factory.CreateSlot e this) slot

    /// Removes slot from cache.
    member this.UnregisterSlot (element: IAttributeSlot): unit =
        unregister slotsPool element

    /// Wraps given node to LanguageModel. Creates new wrapper if needed, otherwise returns cached copy.
    member this.WrapModel (model: IAttributeModel): ILanguageModel =
        wrap modelsPool (fun e -> factory.CreateModel e this) model

    /// Removes model from cache.
    member this.UnregisterModel (model: IAttributeModel): unit =
        unregister modelsPool model

    /// Wraps given node to LanguageEnumeration. Creates new wrapper if needed, otherwise returns cached copy.
    member this.WrapEnumeration (element: IAttributeElement): ILanguageEnumeration =
        wrap enumsPool (fun e -> factory.CreateEnumeration e this) element

    /// Removes model from cache.
    member this.UnregisterEnumeration (element: IAttributeElement): unit =
        unregister enumsPool element

    /// Clears cached values, invalidating all references to Core elements.
    member this.Clear () =
        elementsPool.Clear ()
        modelsPool.Clear ()
        attributesPool.Clear ()
        slotsPool.Clear ()
        enumsPool.Clear ()

/// Abstract factory that creates wrapper objects.
and ILanguageFactory =
    /// Creates AttributeElement wrapper by given CoreElement.
    abstract CreateElement: element: IAttributeElement -> pool: LanguagePool -> ILanguageElement

    /// Creates AttributeModel wrapper by given CoreModel.
    abstract CreateModel: model: IAttributeModel -> pool: LanguagePool -> ILanguageModel

    /// Creates AttributeAttribute wrapper by given CoreElement.
    abstract CreateAttribute: attribute: IAttributeAttribute -> pool: LanguagePool -> ILanguageAttribute

    /// Creates AttributeSlot wrapper by given CoreElement.
    abstract CreateSlot: slot: IAttributeSlot -> pool: LanguagePool -> ILanguageSlot

    /// Creates LanguageEnumeration wrapper by given AttributeElement.
    abstract CreateEnumeration: element: IAttributeElement -> pool: LanguagePool -> ILanguageEnumeration
