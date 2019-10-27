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

namespace Repo.InfrastructureMetamodel.Details.Elements

open Repo.InfrastructureMetamodel
open Repo.LanguageMetamodel

open System.Collections.Generic

/// Cache for wrapper objects. Allows to request wrapper object, store wrapped object, remove wrapped object.
/// Does not provide any consistency checks.
type InfrastructurePool(factory: IInfrastructureFactory) =
    let elementsPool = Dictionary<ILanguageElement, IInfrastructureElement>() :> IDictionary<_, _>
    let modelsPool = Dictionary<ILanguageModel, IInfrastructureModel>() :> IDictionary<_, _>
    let attributesPool = Dictionary<ILanguageAttribute, IInfrastructureAttribute>() :> IDictionary<_, _>
    let slotsPool = Dictionary<ILanguageSlot, IInfrastructureSlot>() :> IDictionary<_, _>
    let enumsPool = Dictionary<ILanguageEnumeration, IInfrastructureEnumeration>() :> IDictionary<_, _>

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
    member this.Wrap (element: ILanguageElement): IInfrastructureElement =
        wrap elementsPool (fun e -> factory.CreateElement e this) element

    /// Removes element from cache.
    member this.UnregisterElement (element: ILanguageElement): unit =
        unregister elementsPool element

    /// Wraps given CoreElement to AttributeAttribute. Creates new wrapper if needed, otherwise returns cached copy.
    member this.WrapAttribute (attribute: ILanguageAttribute): IInfrastructureAttribute =
        wrap attributesPool (fun e -> factory.CreateAttribute e this) attribute

    /// Removes attribute from cache.
    member this.UnregisterAttribute (element: ILanguageAttribute): unit =
        unregister attributesPool element

    /// Wraps given CoreElement to AttributeSlot. Creates new wrapper if needed, otherwise returns cached copy.
    member this.WrapSlot (slot: ILanguageSlot): IInfrastructureSlot =
        wrap slotsPool (fun e -> factory.CreateSlot e this) slot

    /// Removes slot from cache.
    member this.UnregisterSlot (element: ILanguageSlot): unit =
        unregister slotsPool element

    /// Wraps given node to LanguageModel. Creates new wrapper if needed, otherwise returns cached copy.
    member this.WrapModel (model: ILanguageModel): IInfrastructureModel =
        wrap modelsPool (fun e -> factory.CreateModel e this) model

    /// Removes model from cache.
    member this.UnregisterModel (model: ILanguageModel): unit =
        unregister modelsPool model

    /// Wraps given node to LanguageEnumeration. Creates new wrapper if needed, otherwise returns cached copy.
    member this.WrapEnumeration (enumeration: ILanguageEnumeration): IInfrastructureEnumeration =
        wrap enumsPool (fun e -> factory.CreateEnumeration e this) enumeration

    /// Removes model from cache.
    member this.UnregisterEnumeration (enumeration: ILanguageEnumeration): unit =
        unregister enumsPool enumeration

    /// Clears cached values, invalidating all references to Core elements.
    member this.Clear () =
        elementsPool.Clear ()
        modelsPool.Clear ()
        attributesPool.Clear ()
        slotsPool.Clear ()
        enumsPool.Clear ()

/// Abstract factory that creates wrapper objects.
and IInfrastructureFactory =
    /// Creates AttributeElement wrapper by given CoreElement.
    abstract CreateElement: element: ILanguageElement -> pool: InfrastructurePool -> IInfrastructureElement

    /// Creates AttributeModel wrapper by given CoreModel.
    abstract CreateModel: model: ILanguageModel -> pool: InfrastructurePool -> IInfrastructureModel

    /// Creates AttributeAttribute wrapper by given CoreElement.
    abstract CreateAttribute: attribute: ILanguageAttribute -> pool: InfrastructurePool -> IInfrastructureAttribute

    /// Creates AttributeSlot wrapper by given CoreElement.
    abstract CreateSlot: slot: ILanguageSlot -> pool: InfrastructurePool -> IInfrastructureSlot

    /// Creates LanguageEnumeration wrapper by given AttributeElement.
    abstract CreateEnumeration: element: ILanguageEnumeration -> pool: InfrastructurePool -> IInfrastructureEnumeration
