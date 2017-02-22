namespace Repo

type Repo =
    interface
        abstract ModelNodes : unit -> NodeInfo seq
        abstract ModelEdges : unit -> EdgeInfo seq
        abstract MetamodelNodes : unit -> NodeInfo seq
        abstract IsEdgeClass : typeId : string -> bool
        abstract Node : id : string -> NodeInfo
        abstract AddNode : typeId : string -> NodeInfo
        abstract AddEdge : typeId : string -> sourceId : string -> targetId : string -> NodeInfo
    end
