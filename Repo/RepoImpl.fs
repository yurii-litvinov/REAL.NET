namespace Repo

open System.Collections.Generic

open GraphMetametamodel
open MetamodelOperations

type private RepoImpl () = 
    let repoGraph, classes = GraphMetametamodel.createM0Model ()

    let toList attrSeq =
        attrSeq |> Seq.fold (fun (acc : List<AttributeInfo>) attr -> acc.Add(attr); acc) (new List<_>())

    interface Repo.Repo with
        member this.ModelNodes () = 
            let metaclass x = if classes.ContainsKey(x) 
                              then 
                                  match classes.[x] with
                                  | Vertex(info, _) when info.name = "Attribute" -> NodeType.Attribute
                                  | _ -> NodeType.Node
                              else NodeType.Node
                            
            repoGraph.Vertices 
            |> Seq.map (fun (info, kind) 
                            -> new NodeInfo(
                                info.id, 
                                info.name, 
                                metaclass (Vertex (info, kind)), 
                                toList <| effectiveAttributes (repoGraph, classes) (info, kind)
                            )
                       )

        member this.ModelEdges () = 
            let edgeType = function
                | Generalization _ -> EdgeType.Generalization
                | Association _ -> EdgeType.Association
                | Attribute _ -> EdgeType.Attribute
                | Type _ -> EdgeType.Type
                | Value _ -> failwith "There shall be no ref values in metametamodel"

            let kind e = 
                let info, kind = e
                kind

            let name n =
                let info, kind = n
                info.name
                
            repoGraph.Edges |> Seq.map (fun e -> new EdgeInfo(System.Guid.NewGuid().ToString(), name e.Source, name e.Target, edgeType (kind e.Tag)))

        member this.MetamodelNodes () =
            let name n =
                let info, kind = n
                info.name

            let potency n =
                let info, kind = n
                info.potency

            repoGraph.Vertices |> Seq.filter (fun x -> potency x > 0 || potency x = -1) |> Seq.map (fun x -> new NodeInfo(name x, name x, NodeType.Node, new List<_> ()))

        member this.Node id =
            (this :> Repo).ModelNodes () |> Seq.filter (fun x -> x.id = id) |> Seq.exactlyOne
