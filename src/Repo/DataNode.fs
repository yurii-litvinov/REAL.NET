namespace RepoExperimental.DataLayer

type DataNode private (name : string, ``class`` : IElement option) =
    inherit DataElement(name, ``class``)
    
    new (name : string) = DataNode(name, None)
    new (name : string, ``class`` : IElement) = DataNode(name, Some ``class``)

    interface INode
