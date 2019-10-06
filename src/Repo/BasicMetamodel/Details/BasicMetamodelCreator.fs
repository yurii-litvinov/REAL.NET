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

/// Initializes repository with Basic Metamodel.
module Repo.BasicMetamodel.Details.BasicMetamodelCreator

open Repo.BasicMetamodel

let createIn(repo: IBasicRepository): unit =
    let nodeNode = repo.CreateNode Consts.node

    let (--->) source (target, name) =
        repo.CreateEdge source target name

    let (--/-->) source target =
        source ---> (target, Consts.instanceOf) |> ignore

    nodeNode --/--> nodeNode

    let (~+) name = 
        let node = repo.CreateNode name
        node --/--> nodeNode
        node

    let elementNode = +Consts.element
    let edgeNode = +Consts.edge
    let stringNode = +Consts.string
    let repositoryNode = +Consts.repository

    let instanceOfMetaedge = repo.CreateEdge elementNode elementNode Consts.instanceOf
    instanceOfMetaedge --/--> edgeNode

    let generalizationEdge = elementNode ---> (elementNode, Consts.generalization)
    generalizationEdge --/--> edgeNode

    let (--|>) source target =
        let edge = source ---> (target, Consts.generalization)
        edge --/--> generalizationEdge

    nodeNode --|> elementNode
    edgeNode --|> elementNode

    let (--->) source (target, name) = 
        let edge = source ---> (target, name) 
        edge --/--> edgeNode

    repositoryNode ---> (elementNode, Consts.elements)
    edgeNode ---> (elementNode, Consts.source)
    edgeNode ---> (elementNode, Consts.target)
    nodeNode ---> (stringNode, Consts.name)
    edgeNode ---> (stringNode, Consts.targetName)

    ()
