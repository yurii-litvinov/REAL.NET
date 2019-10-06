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

open Repo.CoreMetamodel
open Repo.CoreMetamodel.Details.Elements
open Repo.BasicMetamodel

let createIn(repo: ICoreRepository): unit =
    let repo = (repo :?> CoreRepository).UnderlyingRepo

    let nodeMetatype = repo.Node Consts.node
    let generalizationMetatype = (repo.Node Consts.element).OutgoingEdge Consts.generalization
    let edgeMetatype = repo.Node Consts.edge

    let (--/-->) source target =
        repo.CreateEdge source target Consts.instanceOf |> ignore
        ()

    let (--->) source (target, name) =
        let edge = repo.CreateEdge source target name
        edge --/--> edgeMetatype
        ()

    let (~+) name = 
        let node = repo.CreateNode name
        node --/--> nodeMetatype
        node

    let (--|>) source target =
        let edge = repo.CreateEdge source target Consts.generalization
        edge --/--> generalizationMetatype
        ()

    let model = +"CoreMetametamodel::Model"
    let repository = +"CoreMetametamodel::Repository"
    let element = +"CoreMetametamodel::Element"
    let node = +"CoreMetametamodel::Node"
    let edge = +"CoreMetametamodel::Edge"
    let generalization = +"CoreMetametamodel::Generalization"
    let association = +"CoreMetametamodel::Association"
    let instanceOf = +"CoreMetametamodel::InstanceOf"
    let stringNode = +"CoreMetametamodel::String"

    node --|> element
    edge --|> element
    generalization --|> edge
    association --|> edge
    instanceOf --|> edge

    repository ---> (model, "models")
    model ---> (model, "metamodel")
    model ---> (element, "elements")
    model ---> (stringNode, "name")
    element ---> (model, "model")
    edge ---> (element, "source")
    edge ---> (element, "target")
    node ---> (stringNode, "name")
    association ---> (stringNode, "targetName")

    ()
