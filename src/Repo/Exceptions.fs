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

namespace Repo

/// Thrown if trying to delete a model that is a metamodel for some another model in repository.
exception DeletingUsedModel of modelName: string

/// Thrown if model requested by name from the repository was not found. Do not request models by name, enumerate
/// repo.Models instead.
exception ModelNotFoundException of modelName: string

/// Thrown if model requested by name from the repository appears more than once. Do not request models by name,
/// enumerate repo.Models instead.
exception MultipleModelsException of modelName: string

/// Thrown when someone tries to do something with non-existent element.
exception ElementNotFoundException of elementName: string

/// Thrown when there is more than one element with given name.
exception MultipleElementsException of elementName: string

/// Thrown when someone tries to do something with non-existent attribute. Most likely this means internal error in
/// repository.
exception AttributeNotFoundException of attributeName: string

/// Thrown when there is more than one attribute with given name.
exception MultipleAttributesException of attributeName: string

/// Thrown when adding an attribute will result in other attribute being hidden.
exception AmbiguousAttributesException of attributeName: string

/// Thrown if we are trying to do something wrong with a model. Most probably means incorrect model or
/// internal error in repository.
exception InvalidSemanticOperationException of errorMessage: string
