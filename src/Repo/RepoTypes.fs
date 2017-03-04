namespace Repo

open System.Collections.Generic

type Id = string

type EdgeType =
    | Generalization = 0
    | Association = 1
    | Attribute = 2
    | Type = 3

type NodeType =
    | Node = 0
    | Attribute = 1

type AttributeInfo =
    struct
        val name : string
        val attributeType : Id
        val value : string

        new (name, attributeType, value) = { name = name; attributeType = attributeType; value = value }
    end

type EdgeInfo = 
    struct
        val id : Id
        val source : Id
        val target : Id
        val edgeType : EdgeType

        new (id, source, target, edgeType) = { id = id; source = source; target = target; edgeType = edgeType }
    end

type NodeInfo =
    struct
        val id : Id
        val name : string
        val nodeType : NodeType
        val attributes : List<AttributeInfo>

        new (id, name, nodeType, attributes) = { id = id; name = name; nodeType = nodeType; attributes = attributes }
    end
