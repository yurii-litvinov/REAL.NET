module Interpreters.InterpreterHelper

open Repo

module Element =
    let findAllEdgesFrom (model: IModel) (element: IElement) =
        model.Edges |> Seq.filter (fun (e: IEdge) -> e.From = element)
        
    let next (model: IModel) (element: IElement) = findAllEdgesFrom model element |> Seq.exactlyOne
    
    let findAllEdgesTo (model: IModel) (element: IElement) =
        model.Edges |> Seq.filter (fun (e: IEdge) -> e.To = element) 

