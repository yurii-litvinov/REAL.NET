(* Copyright 2017-2019 Yurii Litvinov
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

/// Initializes repository with Core Metametamodel --- a metamodel for Core Metamodel.
/// It is needed because of a Model node --- we want Core Metamodel to be inside its own model, so we need to introduce
/// model on one metalevel and instantiate it on another. Note that Core Metametamodel elements do not belong to
/// any model.
module Repo.CoreMetamodel.Details.CoreMetametamodelCreator

open Repo
open Repo.CoreMetamodel
open Repo.CoreMetamodel.Details.Elements

let createIn(repo: ICoreRepository): unit =
    let repo = (repo :?> CoreRepository).UnderlyingRepo

    let nodeMetatype = repo.Node BasicMetamodel.Consts.node
    let generalizationMetatype = 
        (repo.Node BasicMetamodel.Consts.element).OutgoingEdge BasicMetamodel.Consts.generalization
    let edgeMetatype = repo.Node BasicMetamodel.Consts.edge

    let (--/-->) source target =
        repo.CreateEdge source target Consts.instanceOfEdge |> ignore

    let (--->) source (target, name) =
        let edge = repo.CreateEdge source target name
        edge --/--> edgeMetatype

    let (~+) name = 
        let node = repo.CreateNode name
        node --/--> nodeMetatype
        node

    let (--|>) source target =
        let edge = repo.CreateEdge source target Consts.generalization
        edge --/--> generalizationMetatype

    let model = +Consts.metamodelModel
    let repository = +Consts.metamodelRepository
    let element = +Consts.metamodelElement
    let node = +Consts.metamodelNode
    let edge = +Consts.metamodelEdgeNode
    let generalization = +Consts.metamodelGeneralization
    let association = +Consts.metamodelAssociation
    let instanceOf = +Consts.metamodelInstanceOf
    let stringNode = +Consts.metamodelString

    node --|> element
    edge --|> element
    generalization --|> edge
    association --|> edge
    instanceOf --|> edge

    repository ---> (model, Consts.modelsEdge)
    model ---> (model, Consts.metamodelEdge)
    model ---> (element, Consts.elementsEdge)
    model ---> (stringNode, Consts.nameEdge)
    element ---> (model, Consts.modelEdge)
    edge ---> (element, Consts.sourceEdge)
    edge ---> (element, Consts.targetEdge)
    node ---> (stringNode, Consts.nameEdge)
    association ---> (stringNode, Consts.targetNameEdge)
