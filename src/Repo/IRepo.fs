namespace Repo

/// General interface of repository. Allows to query information about models and elements and to modify
/// the contents of the repository by adding new nodes or edges as instances of some already existing elements.
type IRepo =
    interface
        abstract Models : unit -> string seq
        abstract ModelNodes : modelName : string -> NodeInfo seq
        abstract ModelEdges : modelName : string -> EdgeInfo seq
        abstract MetamodelNodes : modelName : string -> NodeInfo seq
        abstract IsEdgeClass : typeId : string -> bool
        abstract Node : id : string -> NodeInfo
        abstract AddNode : typeId : string -> modelName : string -> NodeInfo
        abstract AddEdge : typeId : string -> sourceId : string -> targetId : string -> modelName : string -> NodeInfo
        abstract NodeType : id : string -> NodeInfo
        abstract EdgeType : id : string -> string
    end

type IMutableRepo =
    interface
        abstract AddNode : name : string -> potency : int -> level : int -> modelName : string -> NodeInfo
        abstract AddAttribute : name : string -> potency : int -> level : int -> value : string -> modelName : string -> NodeInfo
        abstract AddEdge : edgeType : EdgeType -> sourceId : string -> targetId : string -> potency : int -> level : int -> 
            sourceName : string -> sourceMin : int -> sourceMax : int -> targetName : string -> targetMin : int -> targetMax : int -> modelName : string -> EdgeInfo
        abstract AddInstantiationRelation : instanceId : string -> typeId : string -> unit
    end
