namespace RepoExperimental.DataLayer

[<AbstractClass>]
type DataElement(name : string, ``class`` : IElement option) =
    let mutable name = name

    interface IElement with
        member this.Class: IElement = 
            match ``class`` with
            | Some v -> v
            | None -> this :> IElement
        
        member this.Name 
            with get() : string = name
            and set v = name <- v

