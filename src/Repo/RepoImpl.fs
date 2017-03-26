namespace Repo

open System.Collections.Generic

open MetamodelOperations
open QuickGraph

// TODO: Shall be private
type internal RepoImpl (loader : IModelLoader) as this = 
    let repoGraph, classes = (new BidirectionalGraph<_, _> true, new Dictionary<_, _>())

    let toList attrSeq =
        attrSeq |> Seq.fold (fun (acc : List<AttributeInfo>) attr -> acc.Add(attr); acc) (new List<_>())

    do
        loader.LoadInto (this :> IMutableRepo)

    new () =
        RepoImpl(GraphMetametamodel ())

    interface IRepo with
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
                
            repoGraph.Edges |> Seq.map (fun (e : TaggedEdge<_, _>) -> new EdgeInfo((fst e.Tag).Id, name e.Source, name e.Target, edgeType (kind e.Tag)))

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
            (this :> IRepo).ModelNodes () |> Seq.filter (fun x -> x.id = id) |> Seq.exactlyOne

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
            (this :> IRepo).Node instanceProps.Id

        member this.AddEdge typeId sourceId targetId =
            failwith "Not implemented"

        member this.NodeType id =
            let labels =
                repoGraph.Vertices 
                |> Seq.filter (fun (a, _) -> a.Id = id)

            if Seq.isEmpty labels then
                failwith ("Node with id = " + id + " not found in repo")

            let label = labels |> Seq.exactlyOne

            if not (classes.ContainsKey (Vertex label)) then
                failwith ("Node " + (fst label).Name + " does not have type")

            let typeId = classes.[Vertex label]
            match typeId with
            | Vertex nodeId -> (this :> IRepo).Node (fst nodeId).Id
            | _ -> failwith "Node type shall be node"
            
        member this.EdgeType id =
            let labels =
                repoGraph.Edges
                |> Seq.map (fun e -> e.Tag)
                |> Seq.filter (fun (a, _) -> a.Id = id)

            if Seq.isEmpty labels then
                failwith ("Edge with id = " + id + " not found in repo")

            let label = labels |> Seq.exactlyOne

            if not (classes.ContainsKey (ModelElementLabel.Edge label)) then
                failwith ("Edge " + (fst label).Name + " does not have type")

            let typeId = classes.[ModelElementLabel.Edge label]
            match typeId with
            | Vertex node -> (fst node).Id
            | ModelElementLabel.Edge edge -> (fst edge).Id

    interface IMutableRepo with
        member this.AddNode name potency level =
            let vertex = { Id = newId (); Name = name; Potency = potency; Level = level }
            repoGraph.AddVertex (vertex, Node) |> ignore
            (this :> IRepo).Node vertex.Id

        member this.AddAttribute name potency level value =
            let vertex = { Id = newId (); Name = name; Potency = potency; Level = level }
            let attrValue = String value
            repoGraph.AddVertex (vertex, NodeKind.Attribute { Value = attrValue }) |> ignore
            (this :> IRepo).Node vertex.Id

        member this.AddEdge edgeType sourceId targetId potency level sourceName sourceMin sourceMax targetName targetMin targetMax =
            let node id =
                repoGraph.Vertices 
                |> Seq.filter (fun (a, _) -> a.Id = id)
                |> Seq.exactlyOne
                
            let source = node sourceId
            let target = node targetId

            let label =
                match edgeType with
                | EdgeType.Generalization -> { Id = newId(); Name = "Generalization"; Potency = 0; Level = 0 }, EdgeKind.Generalization
                | EdgeType.Association -> 
                    let modelElementAttributes = { Id = newId(); Name = "Association"; Potency = -1; Level = 0 }
                    let associationAttributes = { SourceName = "Source"; SourceMin = 1; SourceMax = 1; TargetName = targetName; TargetMin = targetMin; TargetMax = targetMax }
                    (modelElementAttributes, EdgeKind.Association associationAttributes)
                | EdgeType.Attribute -> { Id = newId(); Name = "Attribute"; Potency = 1; Level = 0 }, EdgeKind.Attribute
                | EdgeType.Type -> { Id = newId(); Name = "Type"; Potency = 0; Level = 0 }, EdgeKind.Type
                | _ -> failwith "Unknown enum value"

            let edge = new TaggedEdge<_, _>(source, target, label)
            repoGraph.AddEdge edge |> ignore

            // TODO: Implement it much simpler when infrastructure metametamodel is settled.
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
                
            edge |> fun e -> new EdgeInfo((fst e.Tag).Id, name e.Source, name e.Target, edgeType (kind e.Tag))

        member this.AddInstantiationRelation instanceId typeId =
            let element id =
                let nodeList = 
                    repoGraph.Vertices 
                    |> Seq.filter (fun (a, _) -> a.Id = id)
                    |> Seq.toList
                
                match nodeList with 
                | [] ->
                    let edgeList = 
                        repoGraph.Edges 
                        |> Seq.filter (fun e -> (fst e.Tag).Id = id)
                        |> Seq.toList
                    match edgeList with
                    | [] -> failwith "No element with given id found"
                    | [e] -> ModelElementLabel.Edge e.Tag
                    | h :: t -> failwith "Duplicate ids"
                | [n] -> ModelElementLabel.Vertex n
                | h :: t -> failwith "Duplicate ids"

            let element' = element instanceId
            let type' = element typeId
            classes.Add(element instanceId, element typeId)
