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
                                  | Vertex(info, _) when info.Name = "Attribute" -> NodeType.Attribute
                                  | _ -> NodeType.Node
                              else NodeType.Node
                            
            repoGraph.Vertices 
            |> Seq.map (fun (info, kind) 
                            -> new NodeInfo(
                                info.Id, 
                                info.Name, 
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
                info.Name
                
            repoGraph.Edges |> Seq.map (fun e -> new EdgeInfo(System.Guid.NewGuid().ToString(), name e.Source, name e.Target, edgeType (kind e.Tag)))

        member this.MetamodelNodes () =
            let id n =
                let info, kind = n
                info.Id

            let name n =
                let info, kind = n
                info.Name

            let potency n =
                let info, kind = n
                info.Potency

            repoGraph.Vertices |> Seq.filter (fun x -> potency x > 0 || potency x = -1) |> Seq.map (fun x -> new NodeInfo(id x, name x, NodeType.Node, new List<_> ()))

        member this.IsEdgeClass typeId =
            let class' = 
                repoGraph.Vertices 
                |> Seq.filter (fun (a, _) -> a.Id = typeId)
                |> Seq.exactlyOne

            let isEdge node =
                let nodeModelElementAttributes, _ = node
                nodeModelElementAttributes.Name = "Relationship"
                
            if isEdge class' then
                true
            else
                followEdges (repoGraph, classes) isGeneralization class' |> Seq.map isEdge |> Seq.forall id

        member this.Node id =
            (this :> Repo).ModelNodes () |> Seq.filter (fun x -> x.id = id) |> Seq.exactlyOne

        member this.AddNode typeId =
            let class' = 
                repoGraph.Vertices 
                |> Seq.filter (fun (a, _) -> a.Id = typeId)
                |> Seq.exactlyOne

            let classAttributes = MetamodelOperations.effectiveAttributeNodes (repoGraph, classes) class'
            
            let defaultValue attr =
                let attributes, _ = attr
                if attributes.Name = "Name" then
                    let info, _ = class'
                    String (info.Name + " instance")
                else
                    String ""

            let instanceDefaultAttributes = classAttributes |> Seq.map (fun attr -> (attr, defaultValue attr)) |> Map.ofSeq
            
            let instance = MetamodelOperations.instance (repoGraph, classes) (Vertex class') instanceDefaultAttributes
            let instanceProps, _ = instance
            (this :> Repo).Node instanceProps.Id

        member this.AddEdge typeId sourceId targetId =
            failwith "Not implemented"
