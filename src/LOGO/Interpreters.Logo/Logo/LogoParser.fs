module Interpreters.Logo.LogoParser

open Interpreters.Parser

open Interpreters.Logo.TurtleCommand

open Repo

open Interpreters

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

    let stringToDouble expr = double expr
    
    let stringToInt expr = int expr

    let findAllEdgesFrom (model: IModel) (element: IElement) =
        model.Edges |> Seq.filter (fun (e: IEdge) -> e.From = element)
        
    let next (model: IModel) (element: IElement) = findAllEdgesFrom model element |> Seq.exactlyOne
    
    let findAllEdgesTo (model: IModel) (element: IElement) =
        model.Edges |> Seq.filter (fun (e: IEdge) -> e.To = element)
        
    
    let hasAttribute name (element: IElement) =
        element.Attributes |> Seq.filter (fun x -> x.Name = name) |> Seq.isEmpty |> not
    
module AvailableParsers =

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
        | Some ({Variables = set; Context = context; Model = model; Element = element} as p) ->
            if (element.Class.Name = "Repeat") then
                let filter (var: Variable) =
                    match var.Meta.PlaceOfCreation with
                    | PlaceOfCreation(_, Some element) -> true
                    | _ -> false
                let edges = Helper.findAllEdgesFrom model element
                let exitOption = edges |> Seq.filter (Helper.hasAttribute "Tag") |> Seq.tryExactlyOne
                match exitOption with
                    | None -> ParserException.raiseWithPlace "No exit found" (PlaceOfCreation(Some model, Some element))
                    | Some exitEdge ->
                        let exit = exitEdge.To
                        let nextElementOption = edges |> Seq.except [exitEdge] |> Seq.tryExactlyOne
                        match nextElementOption with
                        | None -> ParserException.raiseWithPlace "No next element found" (PlaceOfCreation(Some model, Some element))
                        | Some nextElementEdge ->
                            let nextElement = nextElementEdge.To
                            let vars = set.Filter filter
                            if vars.IsEmpty then
                                let countString = Helper.findAttributeValueByName element "Count"
                                let count = countString |> Helper.stringToInt
                                if (count = 0) then
                                    Some {p with Element = exit}
                                else
                                    let i = Variable.createInt "repeatI" count (Some model, Some element)
                                    let newSet = set.Add i
                                    Some {p with Variables = newSet; Element = nextElement}
                            else
                                let countVarOption = vars |> Seq.filter (fun x -> x.Name = "repeatI") |> Seq.tryExactlyOne
                                match countVarOption with
                                | None -> ParserException.raiseWithPlace "No correct count variable found" (PlaceOfCreation(Some model, Some element))
                                | Some ({Value = value} as countVar) ->
                                    match value with
                                    | Regular (Int intVal) ->
                                        if (intVal = 1) then
                                            let newSet = set.Remove countVar
                                            Some {p with Element = exit; Variables = newSet}
                                        else
                                            let newVal = VariableValue.createInt (intVal - 1)
                                            let newSet = set.ChangeValue countVar newVal
                                            Some {p with Element = nextElement; Variables = newSet}
                                    | _ -> None
                else None
                    
open AvailableParsers

let parseMovement: Parser<Context> = parseForward >>+ parseRight >>+ parseBackward >>+ parseLeft

let parseLogo = parseMovement >>+ parseRepeat