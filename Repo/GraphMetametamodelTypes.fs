namespace Repo

open QuickGraph
open System.Collections.Generic

type internal ModelElementAttributes = 
    { 
        Id : Id;
        Name : string;
        Potency : int;
        Level : int 
    }

/// Attribute value is a reference to an instance of attribute type, which shall be present in a model.
/// For pragmatic reasons instances of basic types (int and string) are not proper nodes in a model, but merely string and int values.
and internal AttributeValue =
    | None
    | Ref of VertexLabel
    | Int of int
    | String of string

and internal AttributeAttributes =
    {
        Value : AttributeValue
    }

and internal NodeKind =
    | Node
    | Attribute of AttributeAttributes

and internal VertexLabel =
    ModelElementAttributes * NodeKind

type internal AssociationAttributes =
    {
        SourceName : string;
        SourceMin : int;
        SourceMax : int;
        TargetName : string;
        TargetMin : int;
        TargetMax : int;
    }

type internal EdgeKind = 
    | Generalization
    | Attribute
    | Type
    | Value
    | Association of AssociationAttributes

type internal EdgeLabel = ModelElementAttributes * EdgeKind

type internal ModelElementLabel = 
    | Vertex of VertexLabel
    | Edge of EdgeLabel

type internal RepoRepresentation = BidirectionalGraph<VertexLabel, TaggedEdge<VertexLabel, EdgeLabel>> * Dictionary<ModelElementLabel, ModelElementLabel>

exception internal InstantiationOfAbstractElementException of ModelElementLabel
