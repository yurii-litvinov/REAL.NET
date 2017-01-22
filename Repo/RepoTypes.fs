namespace Repo

type Id = string

type EdgeType =
    | Generalization = 0
    | Association = 1
    | Attribute = 2
    | Type = 3

type NodeType =
    | Node = 0
    | Attribute = 1

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

        new (id, name, nodeType) = { id = id; name = name; nodeType = nodeType }
    end

type PropertyInfo =
    struct
        val propertyType : Id
        val value : string
    end

