namespace Repo

module Repo = 
    open GraphMetametamodel

    type EdgeType =
        | Generalization = 0
        | Association = 1
        | Attribute = 2
        | Type = 3

    type NodeType =
        | Node = 0
        | Attribute = 1

    type EdgeInfo = 
        struct
            val source : string
            val target : string
            val edgeType : EdgeType

            new (source, target, edgeType) = { source = source; target = target; edgeType = edgeType }
        end

    type NodeInfo =
        struct
            val name : string
            val nodeType : NodeType
            new (name, nodeType) = { name = name; nodeType = nodeType }
        end

    type PropertyInfo =
        struct
            val propertyType : string
            val value : string
        end

    let repoGraph, classes = GraphMetametamodel.createM0Model ()

    let ModelNodes () = 
        let metaclass x = if classes.ContainsKey(x) && classes.[x].name = "Attribute" 
                          then NodeType.Attribute
                          else NodeType.Node
        repoGraph.Vertices |> Seq.map (fun x -> new NodeInfo(x.name, metaclass x))

    let ModelEdges () = 
        let edgeType = function
            | Generalization -> EdgeType.Generalization
            | Association (_, _, _) -> EdgeType.Association
            | Attribute -> EdgeType.Attribute
            | Type -> EdgeType.Type

        repoGraph.Edges |> Seq.map (fun e -> new EdgeInfo(e.Source.name, e.Target.name, edgeType e.Tag))

    let MetamodelNodes () =
        repoGraph.Vertices |> Seq.filter (fun x -> x.potency > 0 || x.potency = -1) |> Seq.map (fun x -> x.name)
