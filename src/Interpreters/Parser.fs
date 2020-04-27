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
                        let parser = StringExpressionParser.Create()
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





    
     
    
    
    

