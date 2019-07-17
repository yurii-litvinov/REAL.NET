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

namespace Repo.CoreMetamodel

open Repo.DataLayer

/// Class that allows to instantiate new models based on Core model semantics.
type CoreSemanticsModelCreator private (repo: IDataRepository, modelName: string, metamodel: IDataModel) =
    let coreMetamodel = repo.Model "CoreMetamodel"
    let coreNode = coreMetamodel.Node "Node"
    let association = coreMetamodel.Node "Association"
    let generalization = coreMetamodel.Node "Generalization"

    let model = repo.CreateModel(modelName, metamodel)

    /// Helper function that creates a copy of a given edge in a current model (identifying source and target by name
    /// and assuming they already present in a model).
    let reinstantiateEdge (edge: IDataEdge) =
        let sourceName = (edge.Source.Value :?> IDataNode).Name
        let targetName = (edge.Target.Value :?> IDataNode).Name
        let source = model.Node sourceName
        let target = model.Node targetName

        match edge with 
        | :? IDataGeneralization ->
            model.CreateGeneralization(generalization, source, target) |> ignore
        | :? IDataAssociation as a ->
            model.CreateAssociation(association, source, target, a.TargetName) |> ignore
        | _ -> failwith "Unknown edge type"

    /// Creates a new model in existing repository with Core Metamodel as its metamodel.
    new (repo: IDataRepository, modelName: string) =
        CoreSemanticsModelCreator(repo, modelName, repo.Model "CoreMetamodel")

    /// Creates a new repository with Core Metamodel and creates a new model in it.
    new (modelName: string) =
        let repo = DataRepo() :> IDataRepository
        let build (builder: IModelBuilder) =
            builder.Build repo

        CoreMetamodelBuilder() |> build
        CoreSemanticsModelCreator(repo, modelName, repo.Model "CoreMetamodel")

    /// Adds a node (an instance of CoreMetamodel.Node) into a model.
    member this.AddNode (name: string) =
        this.AddNodeWithClass name coreNode

    /// Adds a node (an instance of given class) to a model.
    member this.AddNodeWithClass (name: string) (``class``: IDataElement) =
        model.CreateNode(name, ``class``)

    /// Adds an association between two elements. Association will be an instance of a CoreMetamodel.Association
    /// node.
    member this.AddAssociation (source: IDataElement) (target: IDataElement) (name: string) =
        model.CreateAssociation(association, source, target, name)

    /// Adds an association between two elements. Association will be an instance of a CoreMetamodel.Generalization
    /// node.
    member this.AddGeneralization (child: IDataNode) (parent: IDataNode) =
        model.CreateGeneralization(generalization, child, parent) |> ignore

    /// Instantiates an association between two given elements using supplied association class as a type of 
    /// resulting edge.
    member this.AddAssociationWithClass (source: IDataNode) (target: IDataNode) (``class``: IDataAssociation) =
        model.CreateAssociation(``class``, source, target, ``class``.TargetName)

    /// Instantiates an association between two given elements. Searches metamodel for association with given name, and
    /// if such association is found (and exactly once), uses it as a class for a new association.
    member this.AddAssociationByName (source: IDataNode) (target: IDataNode) (name: string) =
        let ``class`` = Model.FindAssociation metamodel name
        model.CreateAssociation(``class``, source, target, ``class``.TargetName)

    /// Creates model builder that uses Core Metamodel semantics and has current model as its ontological metamodel
    /// and Core Metamodel as its linguistic metamodel. So instantiations will use this model for instantiated classes,
    /// but Node, String and Association will be from Core Metamodel.
    member this.CreateInstanceModelBuilder (name: string) =
        CoreSemanticsModelCreator(repo, name, model)

    /// Returns model which this builder builds.
    member this.Model with get () = model

    /// Helper operator that adds a node to a model.
    static member (+) (creator: CoreSemanticsModelCreator, name) = creator.AddNode name

    /// Helper operator that adds a generalization between given elements to a model.
    static member (+--|>) (creator: CoreSemanticsModelCreator, (source, target)) = 
        creator.AddGeneralization source target

    /// Helper operator that adds an association between given elements to a model.
    static member (+--->) (creator: CoreSemanticsModelCreator, (source, target, name)) = 
        creator.AddAssociation source target name |> ignore

    /// Instantiates an exact copy of a metamodel in a current model. Supposed to be used to reintroduce linguistic
    /// metatypes at a new metalevel.
    member this.ReinstantiateParentModel () =
        metamodel.Nodes |> Seq.iter (fun node -> this + node.Name |> ignore)
        metamodel.Edges |> Seq.iter reinstantiateEdge
        ()

    /// Returns node by name, if it exists.
    member this.Node name =
        model.Node name
