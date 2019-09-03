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

namespace Repo.InfrastructureMetamodel

open Repo
open Repo.DataLayer

/// Class that allows to instantiate new models based on Attribute Metamodel semantics.
type InfrastructureSemanticsModelBuilder 
        (
        repo: IDataRepository, 
        modelName: string, 
        ontologicalMetamodel: IDataModel
        ) =
    let infrastructureSemantics = InfrastructureMetamodelSemantics(repo)
    let elementSemantics = ElementSemantics(repo)
    let infrastructureMetamodel = repo.Model Consts.infrastructureMetamodel

    let metamodelNode = infrastructureMetamodel.Node "Node"
    let metamodelString = infrastructureMetamodel.Node "String"
    let metamodelAssociation = infrastructureMetamodel.Node "Association"
    let metamodelGeneralization = infrastructureMetamodel.Node "Generalization"

    let metametamodel = repo.Model Consts.infrastructureMetametamodel
    let metametamodelAttribute = metametamodel.Node "Attribute"
    let metametamodelEnumElement = metametamodel.Node "Attribute"

    let findAttributeNode name =
        infrastructureMetamodel.Nodes 
        |> Seq.filter (fun n -> n.Name = name)
        |> Seq.filter (fun n -> n.OntologicalType = (metametamodelAttribute :> IDataElement))
        |> Seq.exactlyOne

    let findEnumElementNode enumName name =
        let enum = infrastructureMetamodel.Node enumName 
        CoreMetamodel.ElementSemantics.OutgoingAssociationsWithTargetName enum "elements"
        |> Seq.map (fun a -> a.Target.Value :?> IDataNode)
        |> Seq.filter (fun n -> n.Name = name)
        |> Seq.exactlyOne

    let metamodelShapeAttribute = findAttributeNode "shape"
    let metamodelIsAbstractAttribute = findAttributeNode "isAbstract"
    let metamodelInstanceMetatypeAttribute = findAttributeNode "instanceMetatype"

    let metamodelTrue = findEnumElementNode Consts.boolean Consts.stringTrue 
    let metamodelFalse = findEnumElementNode Consts.boolean Consts.stringFalse

    let model = repo.CreateModel(modelName, ontologicalMetamodel, infrastructureMetamodel)

    /// Helper function that adds a new attribute to a node.
    let addAttribute (node: IDataNode) (ontologicalType: IDataNode) (defaultValue: IDataNode) (name: string) =
        elementSemantics.AddAttribute node name ontologicalType defaultValue

    /// Creates a new model in existing repository with Attribute Metamodel as its metamodel.
    new (repo: IDataRepository, modelName: string) =
        InfrastructureSemanticsModelBuilder(repo, modelName, repo.Model Consts.infrastructureMetamodel)

    /// Creates a new repository with Core Metamodel and creates a new model in it.
    new (modelName: string) =
        let repo = DataRepo() :> IDataRepository
        let (~+) (builder: IModelCreator) = builder.CreateIn repo

        +Repo.CoreMetamodel.CoreMetamodelCreator()
        +Repo.AttributeMetamodel.AttributeMetamodelCreator()
        +Repo.LanguageMetamodel.LanguageMetamodelCreator()
        +InfrastructureMetametamodelCreator()
        +InfrastructureMetamodelCreator()

        InfrastructureSemanticsModelBuilder(repo, modelName, repo.Model Consts.infrastructureMetamodel)

    /// Adds a node as a linguistic extension (an instance of InfrastructureMetamodel.Node) with given attributes 
    /// (of type InfrastructureMetamodel.String) into a model.
    member this.AddNode (name: string) (attributes: AttributeInfo list) =
        let node = infrastructureSemantics.InstantiateNode model name metamodelNode Map.empty :?> IDataNode

        attributes 
        |> List.iter (fun attr -> addAttribute node attr.Type attr.DefaultValue attr.Name)
        
        node

    /// Instantiates a node of a given class into a model. Uses provided list to instantiate attributes into slots.
    member this.InstantiateNode
            (name: string)
            (ontologicalType: IDataNode)
            (slotsList: List<string * string>) =
        let attributeValues = 
            Map.ofList slotsList
            |> Map.map (fun _ value -> model.CreateNode(value, metamodelString, metamodelString))
        let node = infrastructureSemantics.Instantiate model ontologicalType // attributeValues
        node :?> IDataNode
    
    /// Adds a new string constant to a model.
    member this.AddStringNode name =
        // TODO: Linguisic type shall be set correctly
        this.InstantiateNode name this.String []

    /// Adds a new int constant to a model.
    member this.AddIntNode name =
        match System.Int32.TryParse(name) with
        // TODO: Linguisic type shall be set correctly
        | (true, _ ) -> this.InstantiateNode name this.Int []
        | _ -> failwith "Trying to add int node with non-int value"
        
    /// Adds a new boolean "true" constant to a model.
    member this.AddBooleanTrueNode =
        this.InstantiateNode Consts.stringTrue metamodelTrue []

    /// Adds a new boolean "false" constant to a model.
    member this.AddBooleanFalseNode =
        this.InstantiateNode Consts.stringFalse metamodelFalse []
    (*
    /// Adds a new attribute to a node with AttributeMetamodel.String as a type.
    member this.AddAttribute (node: IDataNode) (name: string) =
        if not <| AttributeSemanticsHelpers.isAttributeAddingPossible elementSemantics node name stringNode then
            failwith <| "Attribute is already present in an element (including its generalization hierarchy) and has "
                + "different type"
        addAttribute node stringNode emptyString name 

    /// Adds a new attribute with given type to a node.
    member this.AddAttributeWithType 
            (node: IDataNode) 
            (ontologicalType: IDataNode) 
            (defaultValue: IDataNode) 
            (name: string) =
        addAttribute node ontologicalType defaultValue name
        *)
    /// Adds an association between two elements. Association will be an instance of an AttributeMetamodel.Association
    /// node.
    member this.AddAssociation (source: IDataElement) (target: IDataElement) (name: string) =
        model.CreateAssociation(metamodelAssociation, metamodelAssociation, source, target, name)

    /// Adds an association between two elements. Association will be an instance of 
    /// an AttributeMetamodel.Generalization node.
    member this.AddGeneralization (child: IDataNode) (parent: IDataNode) =
        model.CreateGeneralization(metamodelGeneralization, metamodelGeneralization, child, parent) |> ignore
        (*
    /// Instantiates an association between two given elements using supplied association class as a type of 
    /// resulting edge.
    member this.InstantiateAssociation 
            (source: IDataNode) 
            (target: IDataNode) 
            (ontologicalType: IDataElement) 
            (slotsList: List<string * string>) =
        let slots = 
            Map.ofList slotsList
            |> Map.map (fun _ value -> model.CreateNode(value, stringNode, stringNode))
        attributeSemantics.InstantiateAssociation model source target ontologicalType slots
    *)
    /// Creates model builder that uses Attribute Metamodel semantics and has current model as its
    /// ontological metamodel and Attribute Metamodel as its linguistic metamodel. So instantiations will use 
    /// this model for instantiated classes, but Node, String and Association will be from Attribute Metamodel.
    member this.CreateInstanceModelBuilder (name: string) =
        InfrastructureSemanticsModelBuilder(repo, name, model)

    /// Returns model which this builder builds.
    member this.Model with get () = model

    /// Returns repository in which the model is being built.
    member this.Repo with get () = repo

    /// Helper operator that adds a linguistic extension node to a model.
    static member (+) (builder: InfrastructureSemanticsModelBuilder, name) = builder.AddNode name []

    /// Helper operator that adds a generalization between given elements to a model.
    static member (+--|>) (builder: InfrastructureSemanticsModelBuilder, (source, target)) = 
        builder.AddGeneralization source target

    /// Helper operator that adds an association between given elements to a model.
    static member (+--->) (builder: InfrastructureSemanticsModelBuilder, (source, target, name)) = 
        builder.AddAssociation source target name |> ignore

    /// Returns node in current model by name, if it exists.
    member this.Node name =
        model.Node name

    /// Returns node in ontological metamodel by name, if it exists.
    member this.MetamodelNode name =
        ontologicalMetamodel.Node name

    /// Returns association from current model by name, if it exists.
    member this.Association name =
        model.Association name

    /// Returns association from ontological metamodel by name, if it exists.
    member this.MetamodelAssociation name =
        ontologicalMetamodel.Association name

    /// Returns a node for Boolean type in Infrastructure metamodel.
    member this.Boolean =
        infrastructureMetamodel.Node Consts.boolean

    /// Returns a node for Int type in Infrastructure metamodel.
    member this.Int =
        infrastructureMetamodel.Node Consts.int

    /// Returns a node for String type in Infrastructure metamodel.
    member this.String =
        infrastructureMetamodel.Node Consts.string
