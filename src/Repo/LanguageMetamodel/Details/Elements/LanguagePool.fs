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
type LanguagePool(factory: ICoreFactory) =
    let elementsPool = Dictionary<IAttributeElement, ILanguageElement>() :> IDictionary<_, _>
    let modelsPool = Dictionary<IAttributeModel, ILanguageModel>() :> IDictionary<_, _>

    /// Wraps given CoreElement to AttributeElement. Creates new wrapper if needed, otherwise returns cached copy.
    member this.Wrap (element: IAttributeElement): ILanguageElement =
        if elementsPool.ContainsKey element then
            elementsPool.[element]
        else 
            let wrapper = factory.CreateElement element this
            elementsPool.Add(element, wrapper)
            wrapper

    /// Removes element from cache.
    member this.UnregisterElement (element: IAttributeElement): unit =
        if not <| elementsPool.Remove element then 
            failwith "Removing non-existent element"

    /// Wraps given node to CoreModel. Creates new wrapper if needed, otherwise returns cached copy.
    member this.WrapModel (model: IAttributeModel): ILanguageModel =
        if modelsPool.ContainsKey model then
            modelsPool.[model]
        else 
            let wrapper = factory.CreateModel model this
            modelsPool.Add(model, wrapper)
            wrapper

    /// Removes model from cache.
    member this.UnregisterModel (model: IAttributeModel): unit =
        if not <| modelsPool.Remove model then 
            failwith "Removing non-existent model"

    /// Clears cached values, invalidating all references to Core elements.
    member this.Clear () =
        elementsPool.Clear ()
        modelsPool.Clear ()

/// Abstract factory that creates wrapper objects.
and ICoreFactory =
    /// Creates AttributeElement wrapper by given CoreElement.
    abstract CreateElement: element: IAttributeElement -> pool: LanguagePool -> ILanguageElement

    /// Creates AttributeModel wrapper by given CoreModel.
    abstract CreateModel: model: IAttributeModel -> pool: LanguagePool -> ILanguageModel
