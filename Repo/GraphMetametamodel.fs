namespace Repo

module GraphMetametamodel =

    type VertexLabel = 
        { name : string;
          potency : int;
          level : int }

    type EdgeLabel = 
        | Generalization
        | Attribute
        | Type
        | Association of string * int * int

    open QuickGraph

    let private repoGraph = new BidirectionalGraph<VertexLabel, TaggedEdge<VertexLabel, EdgeLabel>> true

    let private createEdge label source target = 
        let edge = new TaggedEdge<_, _>(source, target, label)
        repoGraph.AddEdge edge |> ignore

    let private (~+) name = 
        let vertex = { name = name; potency = -1; level = 0 }
        repoGraph.AddVertex vertex |> ignore
        vertex

    let private (---|>) = createEdge Generalization

    let private (--+-->) = createEdge Attribute

    let private (--*-->) = createEdge Type

    let private (--->) source target targetRole = createEdge (Association targetRole) source target

    let createM0Model () =

        let modelElement = +"ModelElement"
        let node = +"Node"
        let attribute = +"Attribute"
        let relationship = +"Relationship"
        let generalization = +"Generalization"
        let association = +"Association"
        let stringType = +"String"
        let intType = +"Int"

        let name = +"Name"
        let potency = +"Potency"
        let level = +"Level"

        let minTarget = +"MinTarget"
        let maxTarget = +"MaxTarget"
        let minSource = +"MinSource"
        let maxSource = +"MaxSource"
        let sourceName = +"SourceName"
        let targetName = +"TargetName"

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

        repoGraph
