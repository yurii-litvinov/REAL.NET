namespace RepoExperimental.SemanticLayer

open RepoExperimental
open RepoExperimental.DataLayer

/// Helper functions for element semantics
module Element =
    let outgoingLinks (model: IModel) element = 
        let outgoingLink: DataLayer.IElement -> bool = 
            function
            | :? DataLayer.IAssociation as a -> a.Source = Some element 
            | _ -> false

        model.Edges
        |> Seq.append model.Metamodel.Edges
        |> Seq.filter outgoingLink 

/// Helper functions for node semantics.
module Node =
    let name (element: IElement) =
        if not <| element :? INode then
            raise InvalidSemanticOperationException
        (element :?> INode).Name
