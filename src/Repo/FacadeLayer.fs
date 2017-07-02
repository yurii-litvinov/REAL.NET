namespace RepoExperimental

type IAttribute =
    interface
        abstract Name : string with get
        
        // TODO: actually attributes can be of elementary types or of more complex types which are themselves are represented
        // im metamodel or model itself. Needs more thinking on how to represent it properly.
        abstract ``Type`` : string with get

        // TODO: attribute values are also not just strings, they are related to property editor capabilities.
        abstract Value : string with get
    end

type IElement =
    interface
    end

type INode =
    interface
        inherit IElement

        // TODO: actually, not needed. Nodes can have no name and names can be modelled as attributes.
        abstract Name : string with get, set

        // TODO: shapes are actually more complex structures than strings.
        abstract Shape : string with get

        abstract Attributes : IAttribute seq with get
    end

type IEdge = 
    interface
        inherit IElement

        abstract Shape : string with get

        abstract From : IElement with get
        abstract To : IElement with get
    end

type IModel =
    interface
        abstract Name : string with get
        abstract Metamodel : IModel with get

        abstract Nodes : INode seq with get
        abstract Edges : IEdge seq with get

        abstract AddElement : ``type`` : IElement -> IElement

        abstract DeleteElement : element : IElement -> unit
    end

type IRepo =
    interface
        abstract Models : IModel seq with get
    end

[<AbstractClass; Sealed>]
type RepoFactory =
    static member CreateRepo () = ()