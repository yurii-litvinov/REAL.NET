namespace RepoExperimental.DataLayer

type DataGeneralization (``class``: IElement, source: IElement option, target: IElement option) =
    inherit DataRelationship("Generalization", Some ``class``, source, target)

    interface IGeneralization
