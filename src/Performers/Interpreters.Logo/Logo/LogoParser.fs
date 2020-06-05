module Interpreters.Logo.LogoParser

open System.Runtime.Remoting.Contexts
open Interpreters.Parser

open Interpreters.Logo.TurtleCommand

open Repo

open Interpreters
open Interpreters.Expressions
open Repo.FacadeLayer

type Context = { Commands: LCommand list} 

module Context =
    let createContext = {Commands = []}

module private Helper =
    let findAttributeValueByName (element: IElement) (name: string) =
        ElementHelper.getAttributeValue name element

    let distanceName = "Distance"
    
    let degreesName = "Degrees"

    let tryExprToDouble env expr =
        let parser = StringExpressionParser.Create()
        match parser.TryParse env expr with
        | Ok (_, v) -> match TypeConversion.tryDouble v with
                       | Some x -> Ok x
                       | _ ->
                           match TypeConversion.tryInt v with
                           | Some x -> (Ok (double x))
                           | _ -> TypeException("Type is not double") :> exn |> Error
        | Error e -> Error e
    
    let tryExprToInt env expr =
        let parser = StringExpressionParser.Create()
        match parser.TryParse env expr with
        | Ok (_, v) -> match TypeConversion.tryInt v with
                       | Some x -> Ok x
                       | _ -> TypeException("Type is not int") :> exn |> Error
        | Error e -> Error e
        
    let findAllEdgesFrom (model: IModel) (element: IElement) = ElementHelper.outgoingEdges model element
        
    let next (model: IModel) (element: IElement) = ElementHelper.next model element
    
    let findAllEdgesTo (model: IModel) (element: IElement) = ElementHelper.incomingEdges model element      
    
    let hasAttribute name (element: IElement) = ElementHelper.hasAttribute name element
    
module AvailableParsers =

    let parseInitialNode (parsing: Parsing<_> Option) =
        match parsing with
        | None -> None
        | Some ({ Model = model; Element = element } as p) ->
            if (element.Class.Name = "InitialNode") then
                match ElementHelper.tryNext model element with
                | None -> ParserException.raiseWithPlace "Can't determine next element from initial node" (PlaceOfCreation(Some model, Some element))
                | Some nextElement -> Some { p with Element = nextElement }
            else None
    
    let parsePenDown (parsing: Parsing<_> Option) =
        match parsing with
        | None -> None
        | Some ({ Context = context; Model = model; Element = element } as p) ->
            if (element.Class.Name = "PenDown") then
                match ElementHelper.tryNext model element with
                | None -> ParserException.raiseWithPlace "Can't determine next element" (PlaceOfCreation(Some model, Some element))
                | Some nextElement ->
                    let command = LPenDown
                    let newContext = {context with Commands = command :: context.Commands}
                    Some { p with Context = newContext; Element = nextElement }
            else None
            
    let parsePenUp (parsing: Parsing<_> Option) =
        match parsing with
        | None -> None
        | Some ({ Context = context; Model = model; Element = element } as p) ->
            if (element.Class.Name = "PenUp") then
                match ElementHelper.tryNext model element with
                | None -> ParserException.raiseWithPlace "Can't determine next element" (PlaceOfCreation(Some model, Some element))
                | Some nextElement ->
                    let command = LPenUp
                    let newContext = {context with Commands = command :: context.Commands}
                    Some { p with Context = newContext; Element = nextElement }
            else None
    
    let parseForward (parsing: Parsing<Context> option) : Parsing<Context> option =
        match parsing with
        | None -> None
        | Some {Variables = set; Context = context; Model = model; Element = element} -> 
            if (element.Class.Name = "Forward") 
                then let distanceString = Helper.findAttributeValueByName element Helper.distanceName
                     let env = EnvironmentOfExpressions.initWithSetAndPlace set (PlaceOfCreation(Some model, Some element))
                     let distanceRes = distanceString |> Helper.tryExprToDouble env 
                     match distanceRes with
                     | Ok distance ->
                         let command = LForward distance
                         let newContext = {context with Commands = command :: context.Commands}
                         let edges = Helper.findAllEdgesFrom model element
                         if Seq.length edges > 1 then None
                         else let edge = Seq.exactlyOne edges
                              let parsed = {Variables = set; Context = newContext; Model = model; Element = edge.To} 
                              parsed |> Some
                     | Error e -> ParserException(e.Message, PlaceOfCreation(Some model, Some element), e) |> raise
                else None

    let parseBackward (parsing: Parsing<Context> option) : Parsing<Context> option =
        match parsing with
        | None -> None
        | Some {Variables = set; Context = context; Model = model; Element = element} -> 
            if (element.Class.Name = "Backward") 
                then let distanceString = Helper.findAttributeValueByName element Helper.distanceName
                     let env = EnvironmentOfExpressions.initWithSetAndPlace set (PlaceOfCreation(Some model, Some element))
                     let distanceRes = distanceString |> Helper.tryExprToDouble env
                     match distanceRes with
                     | Ok distance ->
                         let command = LBackward distance
                         let newContext = {context with Commands = command :: context.Commands}
                         let edges = Helper.findAllEdgesFrom model element
                         if Seq.length edges > 1 then None
                         else let edge = Seq.exactlyOne edges
                              let parsed = {Variables = set; Context = newContext; Model = model; Element = edge.To} 
                              parsed |> Some
                     | Error e -> ParserException.raiseAll e.Message (PlaceOfCreation(Some model, Some element)) e
                else None

    let parseRight (parsing: Parsing<Context> option) : Parsing<Context> option =
        match parsing with
        | None -> None
        | Some {Variables = set; Context = context; Model = model; Element = element} -> 
            if (element.Class.Name = "Right")
                then let degreesString = Helper.findAttributeValueByName element Helper.degreesName
                     let env = EnvironmentOfExpressions.initWithSetAndPlace set (PlaceOfCreation(Some model, Some element))
                     let degreesRes = degreesString |> Helper.tryExprToDouble env 
                     match degreesRes with
                     | Ok degrees ->
                         let command = LRight degrees
                         let newContext = {context with Commands = command :: context.Commands}
                         let edges = Helper.findAllEdgesFrom model element
                         if Seq.length edges > 1 then None
                         else let edge = Seq.exactlyOne edges
                              let parsed = {Variables = set; Context = newContext; Model = model; Element = edge.To} 
                              parsed |> Some
                     | Error e -> ParserException.raiseAll e.Message (PlaceOfCreation(Some model, Some element)) e
            else None
    
    let parseLeft (parsing: Parsing<Context> option) : Parsing<Context> option =
        match parsing with
        | None -> None
        | Some {Variables = set; Context = context; Model = model; Element = element} -> 
            if (element.Class.Name = "Left")
                then let degreesString = Helper.findAttributeValueByName element Helper.degreesName
                     let env = EnvironmentOfExpressions.initWithSetAndPlace set (PlaceOfCreation(Some model, Some element))
                     let degreesRes = degreesString |> Helper.tryExprToDouble env 
                     match degreesRes with
                     | Ok degrees ->
                         let command = LLeft degrees
                         let newContext = {context with Commands = command :: context.Commands}
                         let edges = Helper.findAllEdgesFrom model element
                         if Seq.length edges > 1 then None
                         else let edge = Seq.exactlyOne edges
                              let parsed = {Variables = set; Context = newContext; Model = model; Element = edge.To} 
                              parsed |> Some
                     | Error e -> ParserException.raiseAll e.Message (PlaceOfCreation(Some model, Some element)) e 
            else None

    let parseRepeat = Interpreters.Parser.AvailableParsers.parseRepeat
    
    let parseIfElse = Interpreters.Parser.AvailableParsers.parseIfElse
                
    let parseExpression = Interpreters.Parser.AvailableParsers.parseExpression
                        
open AvailableParsers

let parseMovement: Parser<Context> = parseForward >>+ parseRight >>+ parseBackward >>+ parseLeft

let parseLogo = parseInitialNode >>+ parseMovement >>+ parseRepeat >>+ parseExpression >>+ parsePenDown >>+ parsePenUp >>+ parseIfElse
    