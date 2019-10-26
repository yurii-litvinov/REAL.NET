(* Copyright 2019 Yurii Litvinov
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

/// Initializes repository with Language Metamodel.
module Repo.LanguageMetamodel.Details.LanguageMetamodelCreator

open Repo.LanguageMetamodel
open Repo.LanguageMetamodel.Details.Elements

let createIn(repo: ILanguageRepository): unit =
    let repo = (repo :?> LanguageRepository).UnderlyingRepo

    let model = repo.InstantiateAttributeMetamodel Consts.languageMetamodel

    let (--->) source (target, name) = model.CreateAssociation source target name |> ignore
    let (~+) name = model.CreateNode name
    let (--|>) source target = model.CreateGeneralization source target |> ignore

    let model = +Consts.model
    let repository = +Consts.repository
    let element = +Consts.element
    let node = +Consts.node
    let enumeration = +Consts.enumeration
    let edge = +Consts.edge
    let generalization = +Consts.generalization
    let association = +Consts.association
    let attribute = +Consts.attribute
    let slot = +Consts.slot
    let stringNode = +Consts.string

    node --|> element
    enumeration --|> element
    edge --|> element
    generalization --|> edge
    association --|> edge

    repository ---> (model, Consts.modelsEdge)
    model ---> (model, Consts.metamodelEdge)
    model ---> (element, Consts.elementsEdge)
    model ---> (stringNode, Consts.nameEdge)
    element ---> (model, Consts.modelEdge)
    edge ---> (element, Consts.sourceEdge)
    edge ---> (element, Consts.targetEdge)
    node ---> (stringNode, Consts.nameEdge)
    enumeration ---> (stringNode, Consts.elementsEdge)
    association ---> (stringNode, Consts.targetNameEdge)

    element ---> (attribute, Consts.attributesEdge)
    element ---> (slot, Consts.slotsEdge)
    slot ---> (attribute, Consts.attributeEdge)
    slot ---> (node, Consts.valueEdge)
    attribute ---> (stringNode, Consts.nameEdge)
    attribute ---> (node, Consts.typeEdge)
