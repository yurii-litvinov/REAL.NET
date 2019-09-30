//(* Copyright 2019 Yurii Litvinov
//*
//* Licensed under the Apache License, Version 2.0 (the "License");
//* you may not use this file except in compliance with the License.
//* You may obtain a copy of the License at
//*
//*     http://www.apache.org/licenses/LICENSE-2.0
//*
//* Unless required by applicable law or agreed to in writing, software
//* distributed under the License is distributed on an "AS IS" BASIS,
//* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//* See the License for the specific language governing permissions and
//* limitations under the License. *)

//namespace Repo.CoreMetamodel.Details

//open Repo.DataLayer

///// Class that allows to instantiate new models based on Core model semantics.
//type CoreSemanticsModelBuilder private (repo: IDataRepository, modelName: string, metamodel: IDataModel) =
//    let coreMetamodel = repo.Model "CoreMetamodel"
//    let node = coreMetamodel.Node "Node"
//    let association = coreMetamodel.Node "Association"
//    let generalization = coreMetamodel.Node "Generalization"

//    let model = repo.CreateModel(modelName, metamodel, coreMetamodel)

//    /// Helper function that creates a copy of a given edge in a current model (identifying source and target by name
//    /// and assuming they already present in a model).
//    let reinstantiateEdge (edge: IDataEdge) = CoreSemanticsHelpers.reinstantiateEdge edge model coreMetamodel

//    /// Creates a new model in existing repository with Core Metamodel as its metamodel.
//    new (repo: IDataRepository, modelName: string) =
//        CoreSemanticsModelBuilder(repo, modelName, repo.Model "CoreMetamodel")

//    /// Creates a new repository with Core Metamodel and creates a new model in it.
//    new (modelName: string) =
//        let repo = DataRepo() :> IDataRepository
//        let build (builder: IModelCreator) =
//            builder.CreateIn repo

//        CoreMetamodelCreator() |> build
//        CoreSemanticsModelBuilder(repo, modelName, repo.Model "CoreMetamodel")

//    /// Creates model builder that uses Core Metamodel semantics and has current model as its ontological metamodel
//    /// and Core Metamodel as its linguistic metamodel. So instantiations will use this model for instantiated classes,
//    /// but Node, String and Association will be from Core Metamodel.
//    member this.CreateInstanceModelBuilder (name: string) =
//        CoreSemanticsModelBuilder(repo, name, model)

//    /// Adds a node (an instance of CoreMetamodel.Node) into a model.
//    member this.AddNode (name: string) =
//        this.AddNodeWithOntologicalType name node

//    /// Adds a node (an instance of given class) to a model.
//    member this.AddNodeWithOntologicalType (name: string) (ontologicalType: IDataElement) =
//        model.CreateNode(name, ontologicalType, node)

//    /// Adds an association between two elements. Association will be an instance of a CoreMetamodel.Association
//    /// node.
//    member this.AddAssociation (source: IDataElement) (target: IDataElement) (name: string) =
//        model.CreateAssociation(association, association, source, target, name)

//    /// Adds a generalization between two elements. Generalization will be an instance of a 
//    /// CoreMetamodel.Generalization node. Note that generalization is always a linguistic extension within its model:
//    /// its ontological type is from Core Metamodel.
//    member this.AddGeneralization (child: IDataNode) (parent: IDataNode) =
//        model.CreateGeneralization(generalization, generalization, child, parent) |> ignore

//    /// Instantiates an association between two given elements using supplied association class as a type of 
//    /// resulting edge.
//    member this.AddAssociationWithOntologicalType 
//            (source: IDataNode) 
//            (target: IDataNode) 
//            (ontologicalType: IDataAssociation) =
//        model.CreateAssociation(ontologicalType, association, source, target, ontologicalType.TargetName)

//    /// Instantiates an association between two given elements. Searches ontological metamodel for association with 
//    /// given name, and if such association is found (and exactly once), uses it as an ontological type 
//    /// for a new association.
//    member this.AddAssociationByName (source: IDataNode) (target: IDataNode) (name: string) =
//        let ontologicalType = ModelSemantics.FindAssociation metamodel name
//        model.CreateAssociation(ontologicalType, association, source, target, ontologicalType.TargetName)

//    /// Returns model which this builder builds.
//    member this.Model with get () = model

//    /// Helper operator that adds a node to a model.
//    static member (+) (creator: CoreSemanticsModelBuilder, name) = creator.AddNode name

//    /// Helper operator that adds a generalization between given elements to a model.
//    static member (+--|>) (creator: CoreSemanticsModelBuilder, (source, target)) = 
//        creator.AddGeneralization source target

//    /// Helper operator that adds an association between given elements to a model.
//    static member (+--->) (creator: CoreSemanticsModelBuilder, (source, target, name)) = 
//        creator.AddAssociation source target name |> ignore

//    /// Instantiates an exact copy of a metamodel in a current model. Supposed to be used to reintroduce linguistic
//    /// metatypes at a new metalevel.
//    member this.ReinstantiateParentModel () =
//        metamodel.Nodes |> Seq.iter (fun node -> this + node.Name |> ignore)
//        metamodel.Edges |> Seq.iter reinstantiateEdge
//        ()

//    /// Returns node by name, if it exists.
//    member this.Node name =
//        model.Node name
