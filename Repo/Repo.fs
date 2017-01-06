namespace Repo

open QuickGraph

type RelationshipType =
    | Generalization
    | Association
    | InstanceOf

type RelationshipInfo = RelationshipInfo of string

type ModelElement =
    | Node of string
    | Relationship of RelationshipInfo
    | Attribute

type Repo() = 
    let repoGraph = new BidirectionalGraph<ModelElement, TaggedEdge<ModelElement, RelationshipInfo>> true

    let createMetamodel () =
        let node1 = Node "Node 1"
        let node2 = Node "Node 2"
        repoGraph.AddVertex node1 |> ignore
        repoGraph.AddVertex node2 |> ignore

        let edge = "Edge" |> RelationshipInfo |> Relationship
        repoGraph.AddVertex edge |> ignore
        ()
    do
        createMetamodel ()

    member this.Entities = 
        repoGraph.Vertices |> Seq.choose (function Node n -> Some n | Relationship (RelationshipInfo r) -> Some r | _ -> None)

    member this.IsEdge name =
        repoGraph.Vertices |> Seq.filter (function Relationship (RelationshipInfo r) -> r = name | _ -> false) |> Seq.length |> (=) 1
