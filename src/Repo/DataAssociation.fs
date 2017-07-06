namespace RepoExperimental.DataLayer

type DataAssociation
    (
        name: string
        , ``class``: IElement
        , source: IElement option
        , target: IElement option
        , nameTarget: string
        ) =
    inherit DataRelationship(name, Some ``class``, source, target)

    interface IAssociation with
        member val TargetName: string = nameTarget with get, set
