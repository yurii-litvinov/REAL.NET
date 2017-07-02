namespace RepoExperimental.DataLayer

type IElement =
    interface
        abstract Name : string with get
        abstract Class : IElement with get
    end

type INode =
    interface
        inherit IElement
    end

type IRelationship =
    interface
        inherit IElement
        abstract Source : IElement with get
        abstract Target : IElement with get
    end

type IGeneralization =
    interface
        inherit IRelationship
    end

type IAssociation =
    interface
        inherit IRelationship
        abstract NameTarget : string with get
    end

type IModel =
    interface
        abstract Name : string with get

        abstract CreateNode : name : string -> ``class`` : IElement -> INode
        abstract CreateGeneralization : name : string -> ``class`` : string -> source : IElement -> target : IElement -> unit
        abstract CreateAssociation : name : string -> ``class`` : string -> source : IElement -> target : IElement -> targetName : string -> unit

        abstract Elements : IElement list
        abstract GetElement : name : string -> IElement
        abstract Nodes : INode list
        abstract GetNode : name : string -> INode

        abstract DeleteElement : element : IElement -> unit
    end

type IRepo =
    interface
        abstract Models : IModel list with get
        abstract CreateModel : name : string -> IModel
        abstract DeleteModel : model : IModel -> unit
    end
