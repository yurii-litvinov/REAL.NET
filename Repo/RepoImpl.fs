namespace Repo

open System.Collections.Generic

open GraphMetametamodel

type private RepoImpl () = 
    let repoGraph, classes = GraphMetametamodel.createM0Model ()

    let followEdge edgePredicate node = 
        repoGraph.OutEdges node
        |> Seq.filter edgePredicate
        |> Seq.exactlyOne
        |> fun e -> e.Target

    let attrType a = a |> followEdge (fun e -> match e.Tag with | Type _ -> true | _ -> false) |> fun n -> n.id

    let attrValue attribute node = 
        match attribute.name with
        | "Name" -> node.name
        | "Level" -> string node.level
        | "Potency" -> string node.potency
        | _ -> ""

    let attributes node =
        repoGraph.OutEdges node 
        |> Seq.filter (fun e -> match e.Tag with | Attribute _ -> true | _ -> false)
        |> Seq.map (fun e -> e.Target)
        |> Seq.map (fun attr -> new AttributeInfo(attr.name, attrType attr, attrValue attr node))

    let rec effectiveAttributes node =
        repoGraph.OutEdges node
        |> Seq.filter (fun e -> match e.Tag with | Generalization _ -> true | _ -> false)
        |> Seq.map (fun e -> e.Target)
        |> Seq.collect (fun parent -> effectiveAttributes parent)
        |> Seq.append (attributes node)
        |> Seq.toList

    let toList attrSeq =
        attrSeq |> Seq.fold (fun (acc : List<AttributeInfo>) attr -> acc.Add(attr); acc) (new List<_>())

    interface Repo.Repo with
        member this.ModelNodes () = 
            let metaclass x = if classes.ContainsKey(x) && classes.[x].name = "Attribute" 
                              then NodeType.Attribute
                              else NodeType.Node
                            
            repoGraph.Vertices |> Seq.map (fun x -> new NodeInfo(x.id, x.name, metaclass x, toList <| effectiveAttributes x))

        member this.ModelEdges () = 
            let edgeType = function
                | Generalization _ -> EdgeType.Generalization
                | Association (_, (_, _, _)) -> EdgeType.Association
                | Attribute _ -> EdgeType.Attribute
                | Type _ -> EdgeType.Type

            repoGraph.Edges |> Seq.map (fun e -> new EdgeInfo(System.Guid.NewGuid().ToString(), e.Source.name, e.Target.name, edgeType e.Tag))

        member this.MetamodelNodes () =
            repoGraph.Vertices |> Seq.filter (fun x -> x.potency > 0 || x.potency = -1) |> Seq.map (fun x -> new NodeInfo(x.name, x.name, NodeType.Node, new List<_> ()))

        member this.Node id =
            (this :> Repo).ModelNodes () |> Seq.filter (fun x -> x.id = id) |> Seq.exactlyOne
