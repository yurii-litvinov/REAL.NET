namespace Repo

module Repo = 
    open GraphMetametamodel

    type EdgeInfo = 
        struct
            val source : string
            val target : string
            val edgeType : int

            new (s, t, et) = { source = s; target = t; edgeType = et }
        end

    let repoGraph = GraphMetametamodel.createM0Model ()

    let Nodes () = 
        repoGraph.Vertices |> Seq.map (fun x -> x.name)

    let Edges () = 
        repoGraph.Edges |> Seq.map (fun e -> new EdgeInfo(e.Source.name, e.Target.name, match e.Tag with | Generalization -> 1 | Association (_, _, _) -> 2 ))