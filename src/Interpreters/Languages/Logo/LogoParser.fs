module Languages.Logo.LogoParser

open Interpreters.Parser

open Languages.Logo.TurtleCommand

open Repo

open System

type Context = { Commands: LCommand list; Model: IModel}

    module Helper =
        let findAttributeValueByName (element: IElement) (name: string) =
            let list = element.Attributes
            let e = Seq.find (fun (x: IAttribute) -> x.Name = name) list
            e.StringValue

        let attributeNameToFind = "Expression"

        let stringToDouble x = Double.Parse x

        let findAllEdgesFrom (model: IModel) (element: IElement) =
            model.Edges |> Seq.filter (fun (e: IEdge) -> e.From = element) 
        
        let findAllEdgesTo (model: IModel) (element: IElement) =
            model.Edges |> Seq.filter (fun (e: IEdge) -> e.To = element) 

let parseForward (parsing: Parsing<Context> option) =
    match parsing with
    | None -> None
    | Some (set, context, element) -> 
        if (element.Name = "Forward") 
            then let distance = Helper.findAttributeValueByName element Helper.attributeNameToFind |> Helper.stringToDouble 
                 let command = LForward distance
                 let newContext = {Commands = command :: context.Commands; Model = context.Model}
                 let edges = Helper.findAllEdgesFrom context.Model element
                 if Seq.length edges > 1 then None
                 else let edge = Seq.exactlyOne edges
                      (set, newContext, edge.To) |> Some
            else None