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

//namespace Repo.AttributeMetamodel

//open Repo.DataLayer
//open Repo.AttributeMetamodel.Semantics

///// Class that allows to instantiate new models based on Attribute Metamodel semantics.
//type AttributeSemanticsModelBuilder 
//        (
//        repo: IDataRepository, 
//        modelName: string, 
//        ontologicalMetamodel: IDataModel
//        ) =
//    let attributeMetamodel = repo.Model "AttributeMetamodel"
//    let elementSemantics = ElementSemantics(attributeMetamodel)
//    let attributeSemantics = InstantiationSemantics(attributeMetamodel)
//    let node = attributeMetamodel.Node "Node"
//    let stringNode = attributeMetamodel.Node "String"
//    let association = attributeMetamodel.Node "Association"
//    let generalization = attributeMetamodel.Node "Generalization"

//    let model = repo.CreateModel(modelName, ontologicalMetamodel, attributeMetamodel)

//    /// Creating empty instance of String that will be attribute default value by default.
//    let emptyString = model.CreateNode("", stringNode, stringNode)

//    /// Helper function that creates a copy of a given edge in a current model (identifying source and target by name
//    /// and assuming they already present in a model).
//    let reinstantiateEdge (edge: IDataEdge) = 
//        Repo.CoreMetamodel.CoreSemanticsHelpers.reinstantiateEdge edge model attributeMetamodel

//    /// Helper function that adds a new attribute to a node.
//    let addAttribute (node: IDataNode) (ontologicalType: IDataNode) (defaultValue: IDataNode) (name: string) =
//        elementSemantics.AddAttribute node name ontologicalType defaultValue

//    /// Helper function that instantiates a new node of a given ontological type hoping that this type does not have
//    /// attributes to instantiate.
//    let addNodeWithOntologicalType (name: string) (ontologicalType: IDataElement) (attributes: string list) =
//        let newNode = attributeSemantics.InstantiateNode model name (ontologicalType :?> IDataNode) Map.empty
//        attributes |> List.iter (addAttribute newNode stringNode emptyString)
//        newNode

//    /// Creates a new model in existing repository with Attribute Metamodel as its metamodel.
//    new (repo: IDataRepository, modelName: string) =
//        AttributeSemanticsModelBuilder(repo, modelName, repo.Model "AttributeMetamodel")

//    /// Creates a new repository with Attribute Metamodel and creates a new model in it.
//    new (modelName: string) =
//        let repo = DataRepo() :> IDataRepository
//        let build (builder: IModelCreator) =
//            builder.CreateIn repo

//        Repo.CoreMetamodel.CoreMetamodelCreator() |> build
//        AttributeMetamodelCreator() |> build
//        AttributeSemanticsModelBuilder(repo, modelName, repo.Model "AttributeMetamodel")

//    /// Adds a node (an instance of AttributeMetamodel.Node) with given attributes (of type AttributeMetamodel.String)
//    /// into a model.
//    member this.AddNode (name: string) (attributes: string list) =
//        addNodeWithOntologicalType name node attributes

//    /// Instantiates a node of a given class into a model. Uses provided list to instantiate attributes into slots.
//    member this.InstantiateNode
//            (name: string)
//            (ontologicalType: IDataNode)
//            (slotsList: List<string * string>) =
//        let attributeValues = 
//            Map.ofList slotsList
//            |> Map.map (fun _ value -> model.CreateNode(value, stringNode, stringNode))
//        let node = attributeSemantics.InstantiateNode model name ontologicalType attributeValues
//        node

//    /// Adds a new attribute to a node with AttributeMetamodel.String as a type.
//    member this.AddAttribute (node: IDataNode) (name: string) =
//        if not <| AttributeSemanticsHelpers.isAttributeAddingPossible elementSemantics node name stringNode then
//            failwith <| "Attribute is already present in an element (including its generalization hierarchy) and has "
//                + "different type"
//        addAttribute node stringNode emptyString name 

//    /// Adds a new attribute with given type to a node.
//    member this.AddAttributeWithType 
//            (node: IDataNode) 
//            (ontologicalType: IDataNode) 
//            (defaultValue: IDataNode) 
//            (name: string) =
//        addAttribute node ontologicalType defaultValue name

//    /// Adds an association between two elements. Association will be an instance of an AttributeMetamodel.Association
//    /// node.
//    member this.AddAssociation (source: IDataElement) (target: IDataElement) (name: string) =
//        model.CreateAssociation(association, association, source, target, name)

//    /// Adds an association between two elements. Association will be an instance of 
//    /// an AttributeMetamodel.Generalization node.
//    member this.AddGeneralization (child: IDataNode) (parent: IDataNode) =
//        if not <| AttributeSemanticsHelpers.isGeneralizationPossible elementSemantics child parent then
//            failwith "Generalization relation is not possible between such elements"
//        model.CreateGeneralization(generalization, generalization, child, parent) |> ignore

//    /// Instantiates an association between two given elements using supplied association class as a type of 
//    /// resulting edge.
//    member this.InstantiateAssociation 
//            (source: IDataNode) 
//            (target: IDataNode) 
//            (ontologicalType: IDataElement) 
//            (slotsList: List<string * string>) =
//        let slots = 
//            Map.ofList slotsList
//            |> Map.map (fun _ value -> model.CreateNode(value, stringNode, stringNode))
//        attributeSemantics.InstantiateAssociation model source target ontologicalType slots

//    /// Creates model builder that uses Attribute Metamodel semantics and has current model as its
//    /// ontological metamodel and Attribute Metamodel as its linguistic metamodel. So instantiations will use 
//    /// this model for instantiated classes, but Node, String and Association will be from Attribute Metamodel.
//    member this.CreateInstanceModelBuilder (name: string) =
//        AttributeSemanticsModelBuilder(repo, name, model)

//    /// Returns model which this builder builds.
//    member this.Model with get () = model

//    /// Returns repository in which the model is being built.
//    member this.Repo with get () = repo

//    /// Helper operator that adds a node to a model.
//    static member (+) (builder: AttributeSemanticsModelBuilder, name) = builder.AddNode name []

//    /// Helper operator that adds a generalization between given elements to a model.
//    static member (+--|>) (builder: AttributeSemanticsModelBuilder, (source, target)) = 
//        builder.AddGeneralization source target

//    /// Helper operator that adds an association between given elements to a model.
//    static member (+--->) (builder: AttributeSemanticsModelBuilder, (source, target, name)) = 
//        builder.AddAssociation source target name |> ignore

//    /// Instantiates an exact copy of Attribute Metamodel in a current model. Supposed to be used to reintroduce
//    /// metatypes at a new metalevel.
//    member this.ReinstantiateAttributeMetamodel () =
//        ontologicalMetamodel.Nodes |> Seq.iter (fun node -> this + node.Name |> ignore)
//        ontologicalMetamodel.Edges |> Seq.iter reinstantiateEdge
//        ()

//    /// Returns node in current model by name, if it exists.
//    member this.Node name =
//        model.Node name

//    /// Returns node in ontological metamodel by name, if it exists.
//    member this.MetamodelNode name =
//        ontologicalMetamodel.Node name

//    /// Returns association from current model by name, if it exists.
//    member this.Association name =
//        model.Association name

//    /// Returns association from ontological metamodel by name, if it exists.
//    member this.MetamodelAssociation name =
//        ontologicalMetamodel.Association name
