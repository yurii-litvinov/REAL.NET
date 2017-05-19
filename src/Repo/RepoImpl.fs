namespace Repo

open System.Collections.Generic

open MetamodelOperations
open QuickGraph

// TODO: Shall be private
type internal RepoImpl (loader : IModelLoader) as this = 
    let repoGraph, classes = (new Dictionary<string, BidirectionalGraph<_, _>>(), new Dictionary<_, _>())

    let toList attrSeq =
        attrSeq |> Seq.fold (fun (acc : List<AttributeInfo>) attr -> acc.Add(attr); acc) (new List<_>())

    do
        let mainModelGraph = new BidirectionalGraph<_, _> true
        let mainModelName = "mainModel"
        repoGraph.Add(mainModelName, mainModelGraph)

        loader.LoadInto (this :> IMutableRepo) mainModelName

        let testLoader = new TestModelLoader() :> IModelLoader

        let testModelGraph = new BidirectionalGraph<_, _> true
        let testModelName = "testModel"
        repoGraph.Add(testModelName, testModelGraph)

        testLoader.LoadInto (this :> IMutableRepo) testModelName

    new () =
        RepoImpl(SimplifiedCoreMetametamodel ())

    interface IRepo with
        member this.Models () = 
            repoGraph.Keys :> seq<string>

        member this.ModelNodes modelName = 
            let metaclass x = if classes.ContainsKey(x) 
                              then 
                                  match classes.[x] with
                                  | Vertex(info, _) when info.Name = "Attribute" -> NodeType.Attribute
                                  | _ -> NodeType.Node
                              else NodeType.Node
                            
            repoGraph.[modelName].Vertices 
            |> Seq.map (fun (info, kind) 
                            -> new NodeInfo(
                                info.Id, 
                                info.Name, 
                                metaclass (Vertex (info, kind)), 
                                toList <| effectiveAttributes (repoGraph.[modelName], classes) (info, kind)
                            )
                       )

        member this.ModelEdges modelName = 
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
                
            repoGraph.[modelName].Edges |> Seq.map (fun (e : TaggedEdge<_, _>) -> new EdgeInfo((fst e.Tag).Id, name e.Source, name e.Target, edgeType (kind e.Tag)))

        member this.MetamodelNodes modelName =
            let id n =
                let info, kind = n
                info.Id

            let name n =
                let info, kind = n
                info.Name

            let potency n =
                let info, kind = n
                info.Potency

            repoGraph.[modelName].Vertices |> Seq.filter (fun x -> potency x > 0 || potency x = -1) |> Seq.map (fun x -> new NodeInfo(id x, name x, NodeType.Node, new List<_> ()))

        member this.IsEdgeClass typeId =
            let isEdgeClassInModel modelName = 
                let classList = 
                    repoGraph.[modelName].Vertices 
                    |> Seq.filter (fun (a, _) -> a.Id = typeId)
                    |> Seq.toList

                let isEdge node =
                    let nodeModelElementAttributes, _ = node
                    nodeModelElementAttributes.Name = "Relationship"

                if classList.IsEmpty then
                    false
                else
                    let class' = classList |> Seq.exactlyOne
                    if isEdge class' then
                        true
                    else
                        followEdges (repoGraph.[modelName], classes) isGeneralization class' |> Seq.map isEdge |> Seq.forall id

            repoGraph.Keys |> Seq.exists isEdgeClassInModel

        member this.Node id =
            let nodeIdInModel modelName =
                let nodesWithGivenId = (this :> IRepo).ModelNodes modelName |> Seq.filter (fun x -> x.id = id)
                if (Seq.toList nodesWithGivenId).IsEmpty then
                    Option.None
                else
                    Option.Some (Seq.exactlyOne nodesWithGivenId)

            repoGraph.Keys |> Seq.choose nodeIdInModel |> Seq.exactlyOne

        member this.AddNode typeId modelName =
            let class' = 
                repoGraph.[modelName].Vertices 
                |> Seq.filter (fun (a, _) -> a.Id = typeId)
                |> Seq.exactlyOne

            let classAttributes = MetamodelOperations.effectiveAttributeNodes (repoGraph.[modelName], classes) class'
            
            let defaultValue attr =
                let attributes, _ = attr
                if attributes.Name = "Name" then
                    let info, _ = class'
                    String (info.Name + " instance")
                else
                    String ""

            let instanceDefaultAttributes = classAttributes |> Seq.map (fun attr -> (attr, defaultValue attr)) |> Map.ofSeq
            
            let instance = MetamodelOperations.instance (repoGraph.[modelName], classes) (Vertex class') instanceDefaultAttributes
            let instanceProps, _ = instance
            (this :> IRepo).Node instanceProps.Id

        member this.AddEdge typeId sourceId targetId modelName =
            failwith "Not implemented"

        member this.NodeType id =
            let nodeWithGivenId modelName = 
                let nodesWithGivenId = repoGraph.[modelName].Vertices |> Seq.filter (fun (a, _) -> a.Id = id)
                if (Seq.toList nodesWithGivenId).IsEmpty then
                    Option.None
                else
                    Option.Some (Seq.exactlyOne nodesWithGivenId)
            
            let labels = repoGraph.Keys |> Seq.choose nodeWithGivenId

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
            let edgeWithGivenId modelName = 
                let edgesWithGivenId = repoGraph.[modelName].Edges|> Seq.map (fun e -> e.Tag) |> Seq.filter (fun (a, _) -> a.Id = id)
                if (Seq.toList edgesWithGivenId).IsEmpty then
                    Option.None
                else
                    Option.Some (Seq.exactlyOne edgesWithGivenId)

            let labels = repoGraph.Keys |> Seq.choose edgeWithGivenId

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
        member this.AddNode name potency level modelName =
            let vertex = { Id = newId (); Name = name; Potency = potency; Level = level }
            repoGraph.[modelName].AddVertex (vertex, Node) |> ignore
            (this :> IRepo).Node vertex.Id

        member this.AddAttribute name potency level value modelName =
            let vertex = { Id = newId (); Name = name; Potency = potency; Level = level }
            let attrValue = String value
            repoGraph.[modelName].AddVertex (vertex, NodeKind.Attribute { Value = attrValue }) |> ignore
            (this :> IRepo).Node vertex.Id

        member this.AddEdge edgeType sourceId targetId potency level sourceName sourceMin sourceMax targetName targetMin targetMax modelName =
            let node id =
                repoGraph.[modelName].Vertices 
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
            repoGraph.[modelName].AddEdge edge |> ignore

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
                let elementInModel modelName = 
                    let nodeList = 
                        repoGraph.[modelName].Vertices 
                        |> Seq.filter (fun (a, _) -> a.Id = id)
                    nodeList

                let nodeList = repoGraph.Keys |> Seq.collect elementInModel |> Seq.toList

                let edgeInModel modelName = 
                    let edgeList = 
                        repoGraph.[modelName].Edges 
                        |> Seq.filter (fun e -> (fst e.Tag).Id = id)
                        |> Seq.toList
                    edgeList
                
                match nodeList with 
                | [] ->
                    let edgeList = repoGraph.Keys |> Seq.collect edgeInModel |> Seq.toList
                    match edgeList with
                    | [] -> failwith "No element with given id found"
                    | [e] -> ModelElementLabel.Edge e.Tag
                    | h :: t -> failwith "Duplicate ids"
                | [n] -> ModelElementLabel.Vertex n
                | h :: t -> failwith "Duplicate ids"

            let element' = element instanceId
            let type' = element typeId
            classes.Add(element instanceId, element typeId)
