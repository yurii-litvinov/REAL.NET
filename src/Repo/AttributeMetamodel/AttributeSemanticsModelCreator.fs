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

namespace Repo.AttributeMetamodel

open Repo.DataLayer

/// Class that allows to instantiate new models based on Attribute model semantics.
type AttributeSemanticsModelCreator private (repo: IDataRepository, modelName: string, metamodel: IDataModel) =
    let elementSemantics = Repo.AttributeMetamodel.Element(repo)
    let attributeMetamodel = repo.Model "AttributeMetamodel"
    let attributeSemantics = AttributeSemantics(repo)
    let coreNode = attributeMetamodel.Node "Node"
    let stringNode = attributeMetamodel.Node "String"
    let association = attributeMetamodel.Node "Association"
    let generalization = attributeMetamodel.Node "Generalization"

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
        AttributeSemanticsModelCreator(repo, modelName, repo.Model "AttributeMetamodel")

    /// Creates a new repository with Core Metamodel and creates a new model in it.
    new (modelName: string) =
        let repo = DataRepo() :> IDataRepository
        let build (builder: IModelBuilder) =
            builder.Build repo

        Repo.CoreMetamodel.CoreMetamodelBuilder() |> build
        AttributeMetamodelBuilder() |> build
        AttributeSemanticsModelCreator(repo, modelName, repo.Model "AttributeMetamodel")

    /// Adds a node (an instance of CoreMetamodel.Node) with given attributes into a model.
    /// Attributes do not receive value.
    member this.AddNode (name: string) (attributes: string list) =
        this.AddNodeWithClass name coreNode attributes

    /// Adds a node (an instance of given class) with given attributes to a model.
    /// Attributes do not receive value.
    member this.AddNodeWithClass (name: string) (``class``: IDataElement) (attributes: string list) =
        let node = model.CreateNode(name, ``class``)
        attributes |> List.iter (fun attr -> this.AddAttribute node attr)
        node

    /// Instantiates a node of a given class into a model. Uses provided attribute list to instantiate attributes.
    member this.InstantiateNode (name: string) (``class``: IDataNode) (attributeValuesList: List<string * string>) =
        let attributeValues = Map.ofList attributeValuesList
        let node = attributeSemantics.InstantiateNode model ``class`` attributeValues
        Node.SetName name node
        node

    /// Adds attribute without a value. So it refers to a type of possible values and is supposed to be instantiated
    /// (then a value becomes an instance of a type). The only type in Core metamodel is String, so this method checks
    /// if String already exists in a model and if not, creates a new node for a String type (so there is a 
    /// "Duplication of concepts" antipattern since String has to be reintroduced at each metalevel, but we do not have
    /// deep metamodeling machinery to avoid it in Core Metamodel). Note that String is also a valid value for an 
    /// attribute and we have no means to distinquish between a value and a type, but that's ok, we don't have such 
    /// high-level concepts in Core metamodel anyway, and attributes are no more than links to some nodes.
    ///
    /// Note that adding attribute using this method and later setting attribute value can lead to unexpected results.
    /// TODO: Refine attribute type/value semantics. Actually there are no attributes in Core Metamodel, so this is
    ///       technically not a bug, but still.
    member this.AddAttribute (node: IDataNode) (name: string) =
        let stringNode =
            if not <| model.HasNode "String" then
                model.CreateNode("String", coreNode)
            else
                model.Node "String"
            
        model.CreateAssociation(association, node, stringNode, name) |> ignore

    /// Adds an attribute to a given node that is an instance of String node, with given value.
    member this.AddAttributeWithValue (node: IDataNode) (name: string) (value: string) =
        let attributeNode = model.CreateNode(name, stringNode)
        model.CreateAssociation(association, node, attributeNode, name) |> ignore
        elementSemantics.SetAttributeValue node name value

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
    member this.InstantiateEdge (source: IDataNode) (target: IDataNode) (``class``: IDataAssociation) =
        model.CreateAssociation(``class``, source, target, ``class``.TargetName)

    /// Creates model builder that uses Core Metamodel semantics and has current model as its ontological metamodel
    /// and Core Metamodel as its linguistic metamodel. So instantiations will use this model for instantiated classes,
    /// but Node, String and Association will be from Core Metamodel.
    member this.CreateInstanceModelBuilder (name: string) =
        AttributeSemanticsModelCreator(repo, name, model)

    /// Returns model which this builder builds.
    member this.Model with get () = model

    /// Returns repository in which the model is being built.
    member this.Repo with get () = repo

    /// Helper operator that adds a node to a model.
    static member (+) (creator: AttributeSemanticsModelCreator, name) = creator.AddNode name []

    /// Helper operator that adds a generalization between given elements to a model.
    static member (+--|>) (creator: AttributeSemanticsModelCreator, (source, target)) = 
        creator.AddGeneralization source target

    /// Helper operator that adds an association between given elements to a model.
    static member (+--->) (creator: AttributeSemanticsModelCreator, (source, target, name)) = 
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
