namespace Repo

type Repo =
    interface
        abstract ModelNodes : unit -> NodeInfo seq
        abstract ModelEdges : unit -> EdgeInfo seq
        abstract MetamodelNodes : unit -> NodeInfo seq
        abstract Node : string -> NodeInfo
    end
