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

namespace Repo.CoreModel

open Repo.DataLayer
open Repo.CoreModel

/// Class that allows to instantiate new models based on Core model semantics.
type ModelCreator private (repo: IDataRepository, modelName: string, metamodel: IDataModel) =
    let coreMetamodel = repo.Model "CoreModel"
    let coreSemantic = CoreSemantic(repo)
    let node = coreMetamodel.Node "Node"
    let stringNode = coreMetamodel.Node "String"
    let association = coreMetamodel.Node "Association"

    let model = repo.CreateModel(modelName, metamodel)

    new (modelName: string) =
        let repo = DataRepo() :> IDataRepository
        let build (builder: IModelBuilder) =
            builder.Build repo

        CoreModel() |> build
        ModelCreator(repo, modelName, repo.Model "CoreModel")

    member this.AddNode (name: string) (attributes: string list) =
        let node = model.CreateNode(name, node)
        attributes |> List.iter (fun attr -> this.AddAttribute node attr)
        node

    member this.AddNodeWithClass (name: string) (``class``: IDataElement) (attributes: string list) =
        let node = model.CreateNode(name, ``class``)
        attributes |> List.iter (fun attr -> this.AddAttribute node attr)
        node

    member this.InstantiateNode (name: string) (``class``: IDataNode) (attributeValuesList: List<string * string>) =
        let attributeValues = Map.ofList attributeValuesList
        let node = coreSemantic.InstantiateNode model ``class`` attributeValues
        Node.setName name node
        node

    /// Adds attribute without a value. So it refers to a type of possible values and is supposed to be instantiated
    /// (then a value becomes an instance of a type). The only type in Core metamodel is String, so this method checks
    /// if String already exists in a model and if not, creates a new node for a String type. Note that String is also
    /// a valid value for an attribute and we have no means to distinquish between a value and a type, but that's ok,
    /// we don't have such high-level concepts in Core metamodel anyway, and attributes are no more than links to some 
    /// nodes.
    member this.AddAttribute (node: IDataNode) (name: string) =
        let stringNode =
            if not <| model.HasNode "String" then
                model.CreateNode("String", node)
            else
                model.Node "String"
            
        model.CreateAssociation(association, node, stringNode, name) |> ignore

    member this.AddAttributeWithValue (node: IDataNode) (name: string) (value: string) =
        let attributeNode = model.CreateNode(name, stringNode)
        model.CreateAssociation(association, node, attributeNode, name) |> ignore
        Element.setAttributeValue node name value

    member this.AddAssociation (source: IDataNode) (target: IDataNode) (name: string) =
        model.CreateAssociation(association, source, target, name)

    member this.InstantiateEdge (source: IDataNode) (target: IDataNode) (``class``: IDataAssociation) =
        model.CreateAssociation(``class``, source, target, ``class``.TargetName)

    member this.CreateInstanceModelBuilder (name: string) =
        ModelCreator(repo, name, model)

    member this.Model with get () = model


