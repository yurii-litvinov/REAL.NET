namespace Repo

open QuickGraph
open System.Collections.Generic

type internal VertexLabel = 
    { 
        id : Id;
        name : string;
        potency : int;
        level : int 
    }

type internal EdgeLabel = 
    | Generalization of Id
    | Attribute of Id
    | Type of Id
    | Association of Id * (Id * int * int)

type internal ModelElement = 
    | Vertex of VertexLabel
    | Edge of EdgeLabel

/// Attribute value is a reference to an instance of attribute type, which shall be present in a model.
/// For pragmatic reasons instances of basic types (int and string) are not proper nodes in a model, but merely string and int values.
type internal AttributeValue =
    | None
    | Ref of VertexLabel
    | Int of int
    | String of string

type internal RepoRepresentation = BidirectionalGraph<VertexLabel, TaggedEdge<VertexLabel, EdgeLabel>> * Dictionary<Id, Id>

module internal GraphMetametamodel =

    let private newId () = System.Guid.NewGuid().ToString()

    //let private instance (repo : Repo) (type' : Id) (attributeValues : Map<VertexLabel, AttributeValue>) =
    //    let name = attributeValues |> Seq.find (fun x -> x.Key.name = "Name") |> fun x -> x.Value
    //    let potency = attributeValues |> Seq.find (fun x -> x.Key.name = "Potency") |> fun x -> x.Value
    //    let level = attributeValues |> Seq.find (fun x -> x.Key.name = "Level") |> fun x -> x.Value

    let createM0Model () =
        let repo = (new BidirectionalGraph<_, _> true, new Dictionary<_, _>())
        let repoGraph, classes = repo

        let createEdge label source target = 
            let edge = new TaggedEdge<_, _>(source, target, label)
            repoGraph.AddEdge edge |> ignore

        let createNode name potency = 
            let vertex = { id = newId (); name = name; potency = potency; level = 0 }
            repoGraph.AddVertex vertex |> ignore
            vertex

        let (~+) name = 
            createNode name -1

        let (~-) name = 
            createNode name 0

        let (---|>) = createEdge (Generalization <| newId ())

        let (--+-->) = createEdge (Attribute <| newId ())

        let (--*-->) = createEdge (Type <| newId ())

        let (--->) source target targetRole = createEdge (Association ((newId ()), targetRole)) source target

        let (--@-->) source target =
            classes.Add(source, target)

        let modelElement = -"ModelElement"
        let node = +"Node"
        let attribute = +"Attribute"
        let relationship = -"Relationship"
        let generalization = +"Generalization"
        let association = +"Association"

        let stringType = -"String"
        let intType = -"Int"

        let name = -"Name"
        let potency = -"Potency"
        let level = -"Level"

        let minTarget = -"MinTarget"
        let maxTarget = -"MaxTarget"
        let minSource = -"MinSource"
        let maxSource = -"MaxSource"
        let sourceName = -"SourceName"
        let targetName = -"TargetName"

        minTarget --@--> attribute
        maxTarget --@--> attribute
        minSource --@--> attribute
        maxSource --@--> attribute
        sourceName --@--> attribute
        targetName --@--> attribute

        name --@--> attribute
        potency --@--> attribute
        level --@--> attribute

        stringType --@--> node
        intType --@--> node

        node ---|> modelElement
        attribute ---|> modelElement
        relationship ---|> modelElement
        generalization ---|> relationship
        association ---|> relationship

        (--->) modelElement modelElement ("class", 1, 1)
        (--->) modelElement attribute ("attributes", 0, -1)
        (--->) attribute node ("type", 1, 1)
        (--->) attribute node ("value", 0, 1)
        (--->) relationship modelElement ("source", 1, 1)
        (--->) relationship modelElement ("target", 1, 1)

        modelElement --+--> name
        modelElement --+--> potency
        modelElement --+--> level

        name --*--> stringType
        potency --*--> intType
        level --*--> intType

        association --+--> minSource
        association --+--> maxSource
        association --+--> sourceName
        association --+--> minTarget
        association --+--> maxTarget
        association --+--> targetName

        minSource --*--> intType
        maxSource --*--> intType
        sourceName --*--> stringType
        minTarget --*--> intType
        maxTarget --*--> intType
        targetName --*--> stringType

        (repoGraph, classes)
