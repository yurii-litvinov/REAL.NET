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

/// Initializes repository with Core Metamodel.
module Repo.CoreMetamodel.Details.CoreMetamodelCreator

open Repo.CoreMetamodel
open Repo.CoreMetamodel.Details.Elements

let createIn(repo: ICoreRepository): unit =
    let repo = (repo :?> CoreRepository).UnderlyingRepo

    let nodeMetatype = repo.Node Consts.metamodelNode
    let generalizationMetatype = repo.Node Consts.metamodelGeneralization
    let edgeMetatype = repo.Node Consts.metamodelEdgeNode
    let modelMetatype = repo.Node Consts.metamodelModel
    let elementMetatype = repo.Node Consts.metamodelElement
    let modelEdgeMetatype = elementMetatype.OutgoingEdge Consts.modelEdge
    let elementsEdgeMetatype = modelMetatype.OutgoingEdge Consts.elementsEdge

    let (--/-->) source target =
        repo.CreateEdge source target Consts.instanceOfEdge |> ignore
        ()

    let (++) model element =
        let elementsEdge = repo.CreateEdge model element Consts.elementsEdge
        elementsEdge --/--> elementsEdgeMetatype
        let modelEdge = repo.CreateEdge element model Consts.modelEdge
        modelEdge --/--> modelEdgeMetatype

    let coreMetamodel = repo.CreateNode Consts.coreMetamodel
    coreMetamodel --/--> modelMetatype

    let (--->) source (target, name) =
        let edge = repo.CreateEdge source target name
        edge --/--> edgeMetatype
        coreMetamodel ++ edge
        ()

    let (~+) name = 
        let node = repo.CreateNode name
        node --/--> nodeMetatype
        coreMetamodel ++ node
        node

    let (--|>) source target =
        let edge = repo.CreateEdge source target Consts.generalizationEdge
        edge --/--> generalizationMetatype
        coreMetamodel ++ edge
        ()

    let model = +Consts.model
    let repository = +Consts.repository
    let element = +Consts.element
    let node = +Consts.node
    let edge = +Consts.edge
    let generalization = +Consts.generalization
    let association = +Consts.association
    let instanceOf = +Consts.instanceOfNode
    let stringNode = +Consts.string

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
