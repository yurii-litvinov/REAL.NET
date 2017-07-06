namespace RepoExperimental.DataLayer

[<AbstractClass>]
type DataRelationship (name: string, ``class``: IElement option, source: IElement option, target: IElement option) =
    inherit DataElement(name, ``class``)

    interface IRelationship with
        member val Source = source with get, set
        member val Target = target with get, set
