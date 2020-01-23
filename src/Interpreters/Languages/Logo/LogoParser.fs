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

    module AvailibleParsers =

        open Interpreters

        let parseForward (parsing: Parsing<Context> option) : Parsing<Context> option =
            match parsing with
            | None -> None
            | Some (set, context, element) -> 
                if (element.Class.Name = "Forward") 
                    then let distanceString = Helper.findAttributeValueByName element Helper.attributeNameToFind
                         let distance = distanceString |> Helper.stringToDouble
                         let command = LForward distance
                         let newContext = {Commands = command :: context.Commands; Model = context.Model}
                         let edges = Helper.findAllEdgesFrom context.Model element
                         if Seq.length edges > 1 then None
                         else let edge = Seq.exactlyOne edges
                              (set, newContext, edge.To) |> Some
                    else None

        let parseBackward (parsing: Parsing<Context> option) : Parsing<Context> option =
            match parsing with
            | None -> None
            | Some (set, context, element) -> 
                if (element.Class.Name = "Backward") 
                    then let distanceString = Helper.findAttributeValueByName element Helper.attributeNameToFind
                         let distance = distanceString |> Helper.stringToDouble
                         let command = LBackward distance
                         let newContext = {Commands = command :: context.Commands; Model = context.Model}
                         let edges = Helper.findAllEdgesFrom context.Model element
                         if Seq.length edges > 1 then None
                         else let edge = Seq.exactlyOne edges
                              (set, newContext, edge.To) |> Some
                    else None

        let parseRight (parsing: Parsing<Context> option) : Parsing<Context> option =
            match parsing with
            | None -> None
            | Some(set, context, element) -> 
                if (element.Class.Name = "Right")
                    then let degreesString = Helper.findAttributeValueByName element Helper.attributeNameToFind
                         let degrees = degreesString |> Helper.stringToDouble
                         let command = LRight degrees
                         let newContext = {Commands = command :: context.Commands; Model = context.Model}
                         let edges = Helper.findAllEdgesFrom context.Model element
                         if Seq.length edges > 1 then None
                         else let edge = Seq.exactlyOne edges
                              (set, newContext, edge.To) |> Some
                else None
        
        let parseLeft (parsing: Parsing<Context> option) : Parsing<Context> option =
            match parsing with
            | None -> None
            | Some(set, context, element) -> 
                if (element.Class.Name = "Left")
                    then let degreesString = Helper.findAttributeValueByName element Helper.attributeNameToFind
                         let degrees = degreesString |> Helper.stringToDouble
                         let command = LLeft degrees
                         let newContext = {Commands = command :: context.Commands; Model = context.Model}
                         let edges = Helper.findAllEdgesFrom context.Model element
                         if Seq.length edges > 1 then None
                         else let edge = Seq.exactlyOne edges
                              (set, newContext, edge.To) |> Some
                else None

open AvailibleParsers
open Interpreters

let parseMovement: Parser<Context> = parseForward >>+ parseRight >>+ parseBackward >>+ parseLeft

let parseLogo = parseMovement