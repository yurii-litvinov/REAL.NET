namespace Repo

module GraphMetametamodel =

    type VertexLabel = 
        { name : string;
          instantiability : int;
          level : int }

    type EdgeLabel = 
        | Generalization
        | Association of string * int * int

    open QuickGraph

    let private repoGraph = new BidirectionalGraph<VertexLabel, TaggedEdge<VertexLabel, EdgeLabel>> true

    let private (~+) name = 
        let vertex = { name = name; instantiability = -1; level = 0 }
        repoGraph.AddVertex vertex |> ignore
        vertex

    let private (---|>) source target = 
        let edge = new TaggedEdge<_, _>(source, target, Generalization)
        repoGraph.AddEdge edge |> ignore

    let private (--->) source target targetRole = 
        let edge = new TaggedEdge<_, _>(source, target, Association targetRole)
        repoGraph.AddEdge edge |> ignore

    let createM0Model () =

        let modelElement = +"ModelElement"
        let node = +"Node"
        let attribute = +"Attribute"
        let relationship = +"Relationship"
        let generalization = +"Generalization"
        let association = +"Association"

        node ---|> modelElement
        attribute ---|> modelElement
        relationship ---|> modelElement
        generalization ---|> relationship
        association ---|> relationship

        (--->) modelElement modelElement ("class", 1, 1)
        (--->) modelElement attribute ("attributes", 0, -1)
        (--->) attribute node ("type", 1, 1)
        (--->) attribute node ("value", 0, 1)
        (--->) relationship node ("source", 1, 1)
        (--->) relationship node ("target", 1, 1)

        repoGraph
