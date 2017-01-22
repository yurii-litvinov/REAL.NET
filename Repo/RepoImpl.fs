namespace Repo

open GraphMetametamodel

type private RepoImpl () = 
    let repoGraph, classes = GraphMetametamodel.createM0Model ()

    interface Repo.Repo with
        member this.ModelNodes () = 
            let metaclass x = if classes.ContainsKey(x) && classes.[x].name = "Attribute" 
                              then NodeType.Attribute
                              else NodeType.Node
            repoGraph.Vertices |> Seq.map (fun x -> new NodeInfo(x.name, x.name, metaclass x))

        member this.ModelEdges () = 
            let edgeType = function
                | Generalization -> EdgeType.Generalization
                | Association (_, _, _) -> EdgeType.Association
                | Attribute -> EdgeType.Attribute
                | Type -> EdgeType.Type

            repoGraph.Edges |> Seq.map (fun e -> new EdgeInfo(System.Guid.NewGuid().ToString(), e.Source.name, e.Target.name, edgeType e.Tag))

        member this.MetamodelNodes () =
            repoGraph.Vertices |> Seq.filter (fun x -> x.potency > 0 || x.potency = -1) |> Seq.map (fun x -> new NodeInfo(x.name, x.name, NodeType.Node))
