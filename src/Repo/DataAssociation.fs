namespace RepoExperimental.DataLayer

type DataAssociation (name : string, ``class``: IElement, nameTarget: string) =
    inherit DataRelationship(name, Some ``class``)

    interface IAssociation with
        member val NameTarget: string = nameTarget with get, set
