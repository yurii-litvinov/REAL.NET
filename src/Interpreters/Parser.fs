namespace Interpreters

open Interpreters.Expressions
open Repo

/// State that should be parsed.
type Parsing<'T> = 
    { Variables: IVariableSet; Context: 'T; Model:IModel; Element: IElement }    

module Parsing =

    /// Gets the Variables component from Parsing.
    let variables (p: Parsing<'T>) = p.Variables

    /// Gets the Context component from Parsing.
    let context (p: Parsing<'T>) = p.Context
    
    /// Gets the Model component from Parsing.
    let model (p: Parsing<'T>) = p.Model

    /// Gets the element component from Parsing.
    let element (p: Parsing<'T>) = p.Element

/// Type of function which transforms one Parsing into another.
type Parser<'T> = Parsing<'T> option -> Parsing<'T> option

module Parser =
    /// Combines two parsers: tries to parse using first parser, then second.
    let combine parser1 parser2 = 
        let combine' p1 p2 parsing =
            if parsing = None then None
            else match p1 parsing with
                    | None -> p2 parsing
                    | x -> x
        combine' parser1 parser2

    /// Infix variant of combine.
    let (>>+) parser1 parser2 = combine parser1 parser2
    
    module AvailableParsers =
        module Helper =
            let stringParser = StringExpressionParser.Create()
            
            let tryInt (env: EnvironmentOfExpressions) expr =
                 match stringParser.TryParse env expr with
                 | Ok(_, RegularValue(Int count)) -> Ok count
                 | Ok (_, _) -> Error(TypeException("It's not int") :> exn)
                 | Error e -> Error e
                 
            let tryBool (env: EnvironmentOfExpressions) expr =
                 match stringParser.TryParse env expr with
                 | Ok(_, RegularValue(Bool boolValue)) -> Ok boolValue
                 | Ok (_, _) -> Error(TypeException("It's not int") :> exn)
                 | Error e -> Error e
        
        let parseExpression (parsing: Parsing<_> option) =
            match parsing with
            | None -> None
            | Some ({Variables = set; Element = element; Model = model} as p) ->
                if (element.Class.Name = "Expression") then
                    let attributeName = "ExpressionValue"
                    let expr = ElementHelper.getAttributeValue attributeName element
                    match ElementHelper.tryNext model element with
                    | Some nextElement ->
                        let env = EnvironmentOfExpressions.initWithSetAndPlace set (PlaceOfCreation(Some model, Some element))
                        let parser = Helper.stringParser
                        let exprArr = expr.Split(';')
                        let apply envRes expr =
                            match envRes with
                            | Error e -> Error e
                            | Ok env ->
                                match parser.TryParse env expr with
                                | Ok (newEnv, _) -> Ok newEnv
                                | Error e -> Error e
                        let res = Array.fold apply (Ok env) exprArr 
                        match res with
                        | Ok newEnv ->
                            Some { p with Element = nextElement; Variables = newEnv.Variables }
                        | Error e -> ParserException.raiseAll e.Message (PlaceOfCreation(Some model, Some element)) e
                    | None -> ParserException.raiseWithPlace "No next element found" (PlaceOfCreation(Some model, Some element))
                else None
                
        let parseRepeat (parsing: Parsing<_> option) =
            match parsing with
            | None -> None
            | Some ({Variables = set; Context = context; Model = model; Element = element} as p) ->
                if (element.Class.Name = "Repeat") then
                    let filter (var: Variable) =
                        match var.Meta.PlaceOfCreation with
                        | PlaceOfCreation(_, Some e) when e = element -> true
                        | _ -> false
                    let edges = ElementHelper.outgoingEdges model element
                    let exitOption = edges |> Seq.filter (ElementHelper.hasAttribute "Tag") |> Seq.tryExactlyOne
                    match exitOption with
                        | None -> ParserException.raiseWithPlace "No exit found" (PlaceOfCreation(Some model, Some element))
                        | Some exitEdge ->
                            let exit = exitEdge.To
                            let nextElementOption = edges |> Seq.except [exitEdge] |> Seq.tryExactlyOne
                            match nextElementOption with
                            | None -> ParserException.raiseWithPlace "Can't determine next element" (PlaceOfCreation(Some model, Some element))
                            | Some nextElementEdge ->
                                let nextElement = nextElementEdge.To
                                let vars = set.Filter filter
                                if vars.IsEmpty then
                                    let countString = ElementHelper.getAttributeValue "Count" element 
                                    let env = EnvironmentOfExpressions.initWithSetAndPlace set (PlaceOfCreation(Some model, Some element))
                                    let countRes = countString |> Helper.tryInt env 
                                    match countRes with
                                    | Ok count ->
                                        if (count = 0) then
                                            Some {p with Element = exit}
                                        else
                                            let i = Variable.createInt "repeatI" count (Some model, Some element)
                                            let newSet = set.Add i
                                            Some {p with Variables = newSet; Element = nextElement}
                                    | Error e -> ParserException.raiseAll e.Message (PlaceOfCreation(Some model, Some element)) e
                                else
                                    let countVarOption = vars |> Seq.filter (fun x -> x.Name = "repeatI") |> Seq.tryExactlyOne
                                    match countVarOption with
                                    | None -> ParserException.raiseWithPlace "No correct count variable found" (PlaceOfCreation(Some model, Some element))
                                    | Some ({Value = value} as countVar) ->
                                        match value with
                                        | RegularValue (Int intVal) ->
                                            if (intVal = 1) then
                                                let newSet = set.Remove countVar
                                                Some {p with Element = exit; Variables = newSet}
                                            else
                                                let newVal = ExpressionValue.createInt (intVal - 1)
                                                let newSet = set.ChangeValue countVar newVal
                                                Some {p with Element = nextElement; Variables = newSet}
                                        | _ -> None
                    else None
        let parseIfElse (parsing: Parsing<_> option) =
                    match parsing with
                    | None -> None
                    | Some ({Variables = set; Context = context; Model = model; Element = element} as p) ->
                        if (element.Class.Name = "IfElse") then
                            let edges = ElementHelper.outgoingEdges model element
                            let elseOption = edges |> Seq.filter (ElementHelper.hasAttribute "Tag") |> Seq.tryExactlyOne
                            match elseOption with
                                | None -> ParserException.raiseWithPlace "No exit found" (PlaceOfCreation(Some model, Some element))
                                | Some elseOption ->
                                    let elseElement = elseOption.To
                                    let nextElementOption = edges |> Seq.except [elseOption] |> Seq.tryExactlyOne
                                    match nextElementOption with
                                    | None -> ParserException.raiseWithPlace "Can't determine next element" (PlaceOfCreation(Some model, Some element))
                                    | Some nextElementEdge ->
                                        let nextElement = nextElementEdge.To
                                        let attributeName = "ExpressionValue"
                                        let expr = ElementHelper.getAttributeValue attributeName element
                                        let env = EnvironmentOfExpressions.initWithSetAndPlace set (PlaceOfCreation(Some model, Some element))
                                        match Helper.tryBool env expr with
                                        | Error e -> ParserException.raiseAll e.Message (PlaceOfCreation(Some model, Some element)) e
                                        | Ok boolValue ->
                                            if boolValue then Some {p with Element = nextElement}
                                            else Some{p with Element = elseElement}
                            else None





    
     
    
    
    

