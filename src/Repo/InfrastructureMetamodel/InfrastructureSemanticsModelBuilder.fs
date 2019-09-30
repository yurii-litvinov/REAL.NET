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

//namespace Repo.InfrastructureMetamodel

//open Repo
//open Repo.DataLayer

///// Class that allows to instantiate new models based on Infrastructure Metamodel semantics.
//type InfrastructureSemanticsModelBuilder 
//        (
//        repo: IDataRepository, 
//        modelName: string, 
//        ontologicalMetamodel: IDataModel
//        ) =
//    let infrastructureMetamodel = repo.Model Consts.infrastructureMetamodel
//    let instantiationSemantics = Semantics.InstantiationSemantics(infrastructureMetamodel)
//    let elementSemantics = Semantics.ElementSemantics(infrastructureMetamodel)

//    let metamodelNode = infrastructureMetamodel.Node "Node"
//    let metamodelString = infrastructureMetamodel.Node "String"
//    let metamodelAssociation = infrastructureMetamodel.Node "Association"
//    let metamodelGeneralization = infrastructureMetamodel.Node "Generalization"

//    let metametamodel = repo.Model Consts.infrastructureMetametamodel
//    let metametamodelAttribute = metametamodel.Node "Attribute"

//    let model = repo.CreateModel(modelName, ontologicalMetamodel, infrastructureMetamodel)

//    /// Helper function that adds a new attribute to an element.
//    let addAttribute (element: IDataElement) (ontologicalType: IDataNode) (defaultValue: IDataNode) (name: string) =
//        if not <| AttributeMetamodel.AttributeSemanticsHelpers.isAttributeAddingPossible elementSemantics element name ontologicalType then
//            failwith <| "Attribute is already present in an element (including its generalization hierarchy) and has "
//                + "different type"
//        elementSemantics.AddAttribute element name ontologicalType defaultValue

//    let addSlots (slotsList: List<string * string>) =
//        Map.ofList slotsList
//        |> Map.map (fun _ value -> model.CreateNode(value, metamodelString, metamodelString))

//    /// Creates a new model in existing repository with Attribute Metamodel as its metamodel.
//    new (repo: IDataRepository, modelName: string) =
//        InfrastructureSemanticsModelBuilder(repo, modelName, repo.Model Consts.infrastructureMetamodel)

//    /// Creates a new repository with Core Metamodel and creates a new model in it.
//    new (modelName: string) =
//        let repo = DataRepo() :> IDataRepository
//        let (~+) (builder: IModelCreator) = builder.CreateIn repo

//        +Repo.CoreMetamodel.CoreMetamodelCreator()
//        +Repo.AttributeMetamodel.AttributeMetamodelCreator()
//        +Repo.LanguageMetamodel.LanguageMetamodelCreator()
//        +InfrastructureMetametamodelCreator()
//        +InfrastructureMetamodelCreator()

//        InfrastructureSemanticsModelBuilder(repo, modelName, repo.Model Consts.infrastructureMetamodel)

//    /// Adds a node as a linguistic extension (an instance of InfrastructureMetamodel.Node) with given attributes 
//    /// (of type InfrastructureMetamodel.String) into a model.
//    member this.AddNode (name: string) (attributes: AttributeInfo list) =
//        let node = instantiationSemantics.InstantiateElement model name metamodelNode Map.empty :?> IDataNode

//        attributes 
//        |> List.iter (fun attr -> addAttribute node attr.Type attr.DefaultValue attr.Name)
        
//        node

//    /// Instantiates a node of a given class into a model. Uses provided list to instantiate attributes into slots.
//    member this.InstantiateNode
//            (name: string)
//            (ontologicalTypeName: string)
//            (slotsList: List<string * string>) =
//        let ontologicalType = ontologicalMetamodel.Node ontologicalTypeName
//        let slots = addSlots slotsList
//        let node = instantiationSemantics.InstantiateElement model name ontologicalType slots
//        node :?> IDataNode
    
//    /// Adds a new string constant to a model.
//    member this.AddStringNode name =
//        instantiationSemantics.InstantiateString model name

//    /// Adds a new int constant to a model.
//    member this.AddIntNode name =
//        match System.Int32.TryParse(name) with
//        | (true, _ ) -> instantiationSemantics.InstantiateInt model name
//        | _ -> failwith "Trying to add int node with non-int value"
        
//    /// Adds a new boolean "true" constant to a model.
//    member this.AddBooleanTrueNode () =
//        this.InstantiateNode Consts.stringTrue Consts.stringTrue []

//    /// Adds a new boolean "false" constant to a model.
//    member this.AddBooleanFalseNode () =
//        this.InstantiateNode Consts.stringFalse Consts.stringFalse []
    
//    /// Adds a new attribute to an element with AttributeMetamodel.String as a type.
//    member this.AddAttribute (element: IDataElement) (name: string) (defaultValue: string) =
//        let defaultValueNode = this.AddStringNode defaultValue
//        addAttribute element metamodelString defaultValueNode name 

//    /// Adds a new attribute with given type to an element. Works only for types that do not themselves have 
//    /// attributes and have node as instance metatype.
//    member this.AddAttributeWithType 
//            (element: IDataElement) 
//            (name: string) 
//            (ontologicalType: IDataNode) 
//            (defaultValue: string) =
//        let defaultValueNode = instantiationSemantics.InstantiateElement model defaultValue ontologicalType Map.empty 
//                               :?> IDataNode
//        addAttribute element ontologicalType defaultValueNode name
 
//    /// Adds an association between two elements. Association will be an instance of an AttributeMetamodel.Association
//    /// node.
//    member this.AddAssociation (source: IDataElement) (target: IDataElement) (name: string) =
//        model.CreateAssociation(metamodelAssociation, metamodelAssociation, source, target, name)

//    /// Adds an association between two elements. Association will be an instance of 
//    /// an AttributeMetamodel.Generalization node.
//    member this.AddGeneralization (child: IDataNode) (parent: IDataNode) =
//        model.CreateGeneralization(metamodelGeneralization, metamodelGeneralization, child, parent) |> ignore
        
//    /// Instantiates an association between two given elements using supplied association class as a type of
//    /// resulting edge.
//    member this.InstantiateAssociation 
//            (source: IDataNode) 
//            (target: IDataNode) 
//            (ontologicalType: IDataElement) 
//            (slotsList: List<string * string>) =
//        let slots = addSlots slotsList
//        let name = 
//            match ontologicalType with
//            | :? IDataAssociation as a -> a.TargetName
//            | :? IDataNode as n -> n.Name
//            | _ -> failwith "Trying to instantiate something strange"
//        instantiationSemantics.InstantiateAssociation model name source target ontologicalType slots 
//        :?> IDataAssociation

//    /// Sets slot value for a given element.
//    member this.SetSlotValue (element: IDataElement) (slot: string) (value: string) =
//        elementSemantics.SetStringSlotValue slot element value

//    /// Creates model builder that uses Attribute Metamodel semantics and has current model as its
//    /// ontological metamodel and Attribute Metamodel as its linguistic metamodel. So instantiations will use 
//    /// this model for instantiated classes, but Node, String and Association will be from Attribute Metamodel.
//    member this.CreateInstanceModelBuilder (name: string) =
//        InfrastructureSemanticsModelBuilder(repo, name, model)

//    /// Returns model which this builder builds.
//    member this.Model = model

//    /// Returns repository in which the model is being built.
//    member this.Repo = repo

//    /// Return instantiation semantics for this model.
//    member this.InstantiationSemantics = instantiationSemantics

//    /// Helper operator that adds a linguistic extension node to a model.
//    static member (+) (builder: InfrastructureSemanticsModelBuilder, name) = builder.AddNode name []

//    /// Helper operator that adds a generalization between given elements to a model.
//    static member (+--|>) (builder: InfrastructureSemanticsModelBuilder, (source, target)) = 
//        builder.AddGeneralization source target

//    /// Helper operator that adds an association between given elements to a model.
//    static member (+--->) (builder: InfrastructureSemanticsModelBuilder, (source, target, name)) = 
//        builder.AddAssociation source target name |> ignore

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

//    /// Returns a node for Boolean type in Infrastructure metamodel.
//    member this.Boolean =
//        infrastructureMetamodel.Node Consts.boolean

//    /// Returns a node for Int type in Infrastructure metamodel.
//    member this.Int =
//        infrastructureMetamodel.Node Consts.int

//    /// Returns a node for String type in Infrastructure metamodel.
//    member this.String =
//        infrastructureMetamodel.Node Consts.string
