module Interpreters.Logo.LogoParser

open Interpreters.Parser

open Interpreters.Logo.TurtleCommand

open Repo

open System

type Context = { Commands: LCommand list} 

module Context =
    let createContext = {Commands = []}

module private Helper =
    let findAttributeValueByName (element: IElement) (name: string) =
        let list = element.Attributes
        let e = Seq.find (fun (x: IAttribute) -> x.Name = name) list
        e.StringValue

    let distanceName = "Distance"
    
    let degreesName = "Degrees"

    let stringToDouble x = Double.Parse x

    let findAllEdgesFrom (model: IModel) (element: IElement) =
        model.Edges |> Seq.filter (fun (e: IEdge) -> e.From = element)
        
    let next (model: IModel) (element: IElement) = findAllEdgesFrom model element |> Seq.exactlyOne
    
    let findAllEdgesTo (model: IModel) (element: IElement) =
        model.Edges |> Seq.filter (fun (e: IEdge) -> e.To = element) 

module AvailableParsers =

    open Interpreters

    let parseForward (parsing: Parsing<Context> option) : Parsing<Context> option =
        match parsing with
        | None -> None
        | Some {Variables = set; Context = context; Model = model; Element = element} -> 
            if (element.Class.Name = "Forward") 
                then let distanceString = Helper.findAttributeValueByName element Helper.distanceName
                     let distance = distanceString |> Helper.stringToDouble
                     let command = LForward distance
                     let newContext = {context with Commands = command :: context.Commands}
                     let edges = Helper.findAllEdgesFrom model element
                     if Seq.length edges > 1 then None
                     else let edge = Seq.exactlyOne edges
                          let parsed = {Variables = set; Context = newContext; Model = model; Element = edge.To} 
                          parsed |> Some
                else None

    let parseBackward (parsing: Parsing<Context> option) : Parsing<Context> option =
        match parsing with
        | None -> None
        | Some {Variables = set; Context = context; Model = model; Element = element} -> 
            if (element.Class.Name = "Backward") 
                then let distanceString = Helper.findAttributeValueByName element Helper.distanceName
                     let distance = distanceString |> Helper.stringToDouble
                     let command = LBackward distance
                     let newContext = {context with Commands = command :: context.Commands}
                     let edges = Helper.findAllEdgesFrom model element
                     if Seq.length edges > 1 then None
                     else let edge = Seq.exactlyOne edges
                          let parsed = {Variables = set; Context = newContext; Model = model; Element = edge.To} 
                          parsed |> Some
                else None

    let parseRight (parsing: Parsing<Context> option) : Parsing<Context> option =
        match parsing with
        | None -> None
        | Some {Variables = set; Context = context; Model = model; Element = element} -> 
            if (element.Class.Name = "Right")
                then let degreesString = Helper.findAttributeValueByName element Helper.degreesName
                     let degrees = degreesString |> Helper.stringToDouble
                     let command = LRight degrees
                     let newContext = {context with Commands = command :: context.Commands}
                     let edges = Helper.findAllEdgesFrom model element
                     if Seq.length edges > 1 then None
                     else let edge = Seq.exactlyOne edges
                          let parsed = {Variables = set; Context = newContext; Model = model; Element = edge.To} 
                          parsed |> Some
            else None
    
    let parseLeft (parsing: Parsing<Context> option) : Parsing<Context> option =
        match parsing with
        | None -> None
        | Some {Variables = set; Context = context; Model = model; Element = element} -> 
            if (element.Class.Name = "Left")
                then let degreesString = Helper.findAttributeValueByName element Helper.degreesName
                     let degrees = degreesString |> Helper.stringToDouble
                     let command = LLeft degrees
                     let newContext = {context with Commands = command :: context.Commands}
                     let edges = Helper.findAllEdgesFrom model element
                     if Seq.length edges > 1 then None
                     else let edge = Seq.exactlyOne edges
                          let parsed = {Variables = set; Context = newContext; Model = model; Element = edge.To} 
                          parsed |> Some
            else None

    let parseRepeat (parsing: Parsing<Context> option) : Parsing<Context> option =
        match parsing with
        | None -> None
        | Some ({Variables = set; Context = context; Model = model; Element = element} as p) -> ParserException.raiseException "e"
                

open Interpreters
open AvailableParsers

let parseMovement: Parser<Context> = parseForward >>+ parseRight >>+ parseBackward >>+ parseLeft

let parseLogo = parseMovement