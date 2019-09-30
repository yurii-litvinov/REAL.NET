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

//namespace Repo.LanguageMetamodel

//open Repo.DataLayer

///// Class that allows to instantiate new models based on Language Metamodel semantics.
//type LanguageSemanticsModelBuilder 
//        (
//        repo: IDataRepository, 
//        modelName: string, 
//        ontologicalMetamodel: IDataModel
//        ) =
//    let linguisticMetamodel = repo.Model Consts.languageMetamodel
//    let elementSemantics = Semantics.ElementSemantics(linguisticMetamodel)
//    let semantics = Semantics.InstantiationSemantics(linguisticMetamodel)
//    let languageNode = linguisticMetamodel.Node "Node"
//    let languageString = linguisticMetamodel.Node "String"
//    let languageAssociation = linguisticMetamodel.Node "Association"
//    let languageGeneralization = linguisticMetamodel.Node "Generalization"
    
//    let languageAttribute = linguisticMetamodel.Node "Attribute"
//    let attributeAssociation = linguisticMetamodel.Association "attributes"
//    let attributeTypeAssociation = linguisticMetamodel.Association "type"
//    let attributeDefaultValueAssociation = linguisticMetamodel.Association "defaultValue"
    
//    let model = repo.CreateModel(modelName, ontologicalMetamodel, linguisticMetamodel)

//    /// Creating empty instance of String that will be attribute default value by default.
//    let emptyString = model.CreateNode("", languageString, languageString)

//    /// Helper function that creates a copy of a given edge in a current model (identifying source and target by name
//    /// and assuming they already present in a model).
//    let reinstantiateEdge (edge: IDataEdge) =
//        Repo.CoreMetamodel.CoreSemanticsHelpers.reinstantiateEdge edge model linguisticMetamodel

//    /// Helper function that adds a new attribute to a node.
//    let addAttribute (node: IDataNode) (attributeOntologicalType: IDataNode) (defaultValue: IDataNode) (name: string) =
//        let model = Semantics.ElementSemantics.ContainingModel node
//        let attributeNode = model.CreateNode(name, languageAttribute, languageAttribute)
//        model.CreateAssociation
//                (
//                attributeAssociation,
//                attributeAssociation,
//                node,
//                attributeNode,
//                name
//                ) |> ignore

//        model.CreateAssociation
//                (
//                attributeTypeAssociation,
//                attributeTypeAssociation,
//                attributeNode,
//                attributeOntologicalType,
//                "type"
//                ) |> ignore

//        model.CreateAssociation
//                (
//                attributeDefaultValueAssociation,
//                attributeDefaultValueAssociation,
//                attributeNode,
//                defaultValue,
//                "defaultValue"
//                ) |> ignore

//    /// Helper function that instantiates a new node of a given ontological type hoping that this type does not have
//    /// attributes to instantiate.
//    let addNodeWithOntologicalType (name: string) (ontologicalType: IDataElement) (attributes: string list) =
//        let newNode = semantics.InstantiateNode model name (ontologicalType :?> IDataNode) Map.empty
//        attributes |> List.iter (addAttribute newNode languageString emptyString)
//        newNode

//    /// Reinstantiates a node as an instance of Language Metamodel.
//    let reinstantiateNode (node: IDataNode) =
//        model.CreateNode(node.Name, languageNode, languageNode) |> ignore

//    /// Creates a new model in existing repository with Language Metamodel as its metamodel.
//    new (repo: IDataRepository, modelName: string) =
//        LanguageSemanticsModelBuilder(repo, modelName, repo.Model Consts.languageMetamodel)

//    /// Creates a new repository with Language Metamodel and creates a new model in it.
//    new (modelName: string) =
//        let repo = DataRepo() :> IDataRepository
//        let (~+) (builder: IModelCreator) = builder.CreateIn repo

//        +Repo.CoreMetamodel.CoreMetamodelCreator()
//        +Repo.AttributeMetamodel.AttributeMetamodelCreator()
//        +LanguageMetamodelCreator()

//        LanguageSemanticsModelBuilder(repo, modelName, repo.Model Consts.languageMetamodel)

//    /// Adds a node (an instance of LanguageMetamodel.Node) with given attributes (of type LanguageMetamodel.String)
//    /// with empty default values into a model.
//    member this.AddNode (name: string) (attributes: string list) =
//        addNodeWithOntologicalType name languageNode attributes

//    /// Instantiates a node of a given class into a model. Uses provided list to instantiate attributes into slots.
//    member this.InstantiateNode
//            (name: string)
//            (ontologicalType: IDataNode)
//            (slotsList: List<string * string>) =
//        let attributeValues = 
//            Map.ofList slotsList
//            |> Map.map (fun _ value -> model.CreateNode(value, languageString, languageString))
//        let node = semantics.InstantiateNode model name ontologicalType attributeValues
//        node

//    /// Adds a new attribute to a node with AttributeMetamodel.String as a type.
//    member this.AddAttribute (node: IDataNode) (name: string) =
//        addAttribute node languageString emptyString name 

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
//        model.CreateAssociation(languageAssociation, languageAssociation, source, target, name)

//    /// Adds an association between two elements. Association will be an instance of 
//    /// an AttributeMetamodel.Generalization node.
//    member this.AddGeneralization (child: IDataNode) (parent: IDataNode) =
//        let isGeneralizationPossible = 
//            Repo.AttributeMetamodel.AttributeSemanticsHelpers.isGeneralizationPossible elementSemantics child parent
//        if not <| isGeneralizationPossible then
//            failwith "Generalization relation is not possible between such elements"
//        model.CreateGeneralization(languageGeneralization, languageGeneralization, child, parent) |> ignore

//    /// Instantiates an association between two given elements using supplied association class as a type of 
//    /// resulting edge.
//    member this.InstantiateAssociation 
//            (source: IDataNode) 
//            (target: IDataNode) 
//            (ontologicalType: IDataElement) 
//            (slotsList: List<string * string>) =
//        let slots = 
//            Map.ofList slotsList
//            |> Map.map (fun _ value -> model.CreateNode(value, languageString, languageString))
//        semantics.InstantiateAssociation model source target ontologicalType slots

//    /// Creates a new enumeration with given literals.
//    member this.AddEnum name literals = 
//        let enum = this.InstantiateNode name (linguisticMetamodel.Node "Enum") []
//        literals |> Seq.iter (fun l ->
//            let enumLiteral = this.InstantiateNode l (linguisticMetamodel.Node "String") []
//            let metamodelEnumLiteralLink = Semantics.ModelSemantics.FindAssociation linguisticMetamodel "elements"
//            let association = this.InstantiateAssociation enum enumLiteral metamodelEnumLiteralLink []
//            association.TargetName <- "elements"
//        )
//        enum

//    /// Creates model builder that uses Language Metamodel semantics and has current model as its
//    /// ontological metamodel and Language Metamodel as its linguistic metamodel. So instantiations will use 
//    /// this model for instantiated classes, but Node, String and Association will be from Language Metamodel.
//    member this.CreateInstanceModelBuilder (name: string) =
//        LanguageSemanticsModelBuilder(repo, name, model)

//    /// Returns model which this builder builds.
//    member this.Model with get () = model

//    /// Returns repository in which the model is being built.
//    member this.Repo with get () = repo

//    /// Helper operator that adds a node to a model.
//    static member (+) (builder: LanguageSemanticsModelBuilder, name) = builder.AddNode name []

//    /// Helper operator that adds a generalization between given elements to a model.
//    static member (+--|>) (builder: LanguageSemanticsModelBuilder, (source, target)) = 
//        builder.AddGeneralization source target

//    /// Helper operator that adds an association between given elements to a model.
//    static member (+--->) (builder: LanguageSemanticsModelBuilder, (source, target, name)) = 
//        builder.AddAssociation source target name |> ignore

//    /// Instantiates an exact copy of Language Metamodel in a current model. Supposed to be used to reintroduce
//    /// metatypes at a new metalevel. Does not reinstantiate already existing (by name) nodes.
//    member this.ReinstantiateLanguageMetamodel () =
//        ontologicalMetamodel.Nodes 
//        |> Seq.iter (fun node -> if not <| this.Model.HasNode node.Name then reinstantiateNode node |> ignore)

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
