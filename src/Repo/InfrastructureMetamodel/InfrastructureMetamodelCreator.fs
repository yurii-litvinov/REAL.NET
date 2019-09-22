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

namespace Repo.InfrastructureMetamodel

open Repo
open Repo.DataLayer
open Repo.LanguageMetamodel

/// Initializes repository with Infrastructure Metamodel, which defines a metamodel that can be instantiated by actual
/// models (or metamodels). Editor can not show this model directly (only in palette), but can work with all models 
/// that instantiate (directly or indirectly) this metamodel. So, it is a kind of built-in metaeditor.
type InfrastructureMetamodelCreator() =
    interface IModelCreator with
        member this.CreateIn(repo: IDataRepository): unit =
            let metamodel = repo.Model Consts.infrastructureMetametamodel
            let model = repo.CreateModel(Consts.infrastructureMetamodel, metamodel, metamodel)
            let elementSemantics = LanguageMetamodel.ElementSemantics metamodel

            let metamodelBoolean = metamodel.Node Consts.boolean
            let metamodelTrue = EnumSemantics.EnumElementNode metamodelBoolean Consts.stringTrue 

            let modelTrue = model.CreateNode("true", metamodelTrue, metamodel.Node Consts.boolean)

            Reinstantiator.reinstantiateInfrastructureMetametamodel repo model

            elementSemantics.SetSlotValue (model.Node "Element") "isAbstract" modelTrue

            ()
