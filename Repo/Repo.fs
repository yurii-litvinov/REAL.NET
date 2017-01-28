namespace Repo

type Repo =
    interface
        abstract ModelNodes : unit -> NodeInfo seq
        abstract ModelEdges : unit -> EdgeInfo seq
        abstract MetamodelNodes : unit -> NodeInfo seq
        abstract Node : id : string -> NodeInfo
        abstract AddNode : typeId : string -> NodeInfo
        abstract AddEdge : sourceId : string -> targetId : string -> typeId : string -> NodeInfo
    end
