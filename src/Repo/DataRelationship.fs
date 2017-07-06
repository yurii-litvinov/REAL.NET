namespace RepoExperimental.DataLayer

[<AbstractClass>]
type DataRelationship (name: string, ``class``: IElement option) =
    inherit DataElement(name, ``class``)

    let mutable source = None
    let mutable target = None

    interface IRelationship with
        member this.Source
            with get (): IElement option = 
                source
            and set (v: IElement option): unit = 
                source <- v

        member this.Target
            with get (): IElement option = 
                target
            and set (v: IElement option): unit = 
                target <- v
