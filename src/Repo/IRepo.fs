namespace Repo

/// General interface of repository. Allows to query information about models and elements and to modify
/// the contents of the repository by adding new nodes or edges as instances of some already existing elements.
type IRepo =
    interface
        abstract ModelNodes : unit -> NodeInfo seq
        abstract ModelEdges : unit -> EdgeInfo seq
        abstract MetamodelNodes : unit -> NodeInfo seq
        abstract IsEdgeClass : typeId : string -> bool
        abstract Node : id : string -> NodeInfo
        abstract AddNode : typeId : string -> NodeInfo
        abstract AddEdge : typeId : string -> sourceId : string -> targetId : string -> NodeInfo
    end

type IMutableRepo =
    interface
        abstract AddNode : name : string -> potency : int -> level : int -> NodeInfo
        abstract AddAttribute : name : string -> potency : int -> level : int -> value : string -> NodeInfo
        abstract AddEdge : edgeType : EdgeType -> sourceId : string -> targetId : string -> potency : int -> level : int -> 
            sourceName : string -> sourceMin : int -> sourceMax : int -> targetName : string -> targetMin : int -> targetMax : int -> EdgeInfo
        abstract AddInstantiationRelation : instanceId : string -> typeId : string -> unit
    end