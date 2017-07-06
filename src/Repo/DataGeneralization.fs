namespace RepoExperimental.DataLayer

type DataGeneralization private (``class``: IElement) =
    inherit DataRelationship("Generalization", Some ``class``)

    interface IGeneralization
