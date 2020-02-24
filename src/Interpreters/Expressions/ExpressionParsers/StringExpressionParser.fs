module Interpreters.Expressions.ExpressionParsers

open System
open System.Text.RegularExpressions
open Interpreters

/// Combines two parsers: tries to parse using first parser, then second.
let combine parser1 parser2 =
    let combine' p1 p2 (parsing: string): VariableValue option =
        match p1 parsing with
        | None -> p2 parsing
        | x -> x
    combine' parser1 parser2

/// Infix variant of combine.
let (>>+) parser1 parser2 = combine parser1 parser2

module Helper =

    let parseDouble (expr: string) =
        try
            expr
            |> double
            |> VariableValue.createDouble
            |> Some
        with :? FormatException -> None

    let parseInt (expr: string) =
        try
            expr
            |> int
            |> VariableValue.createInt
            |> Some
        with :? FormatException -> None

    let parser = (parseInt >>+ parseDouble)

open Helper

type StringExpressionParser() =
    class
        interface IStringExpressionParser with
            member this.TryParse set stringExpression = parser stringExpression
            
            member this.Parse set stringExpression =
                let parsed = (this :> IStringExpressionParser).TryParse set stringExpression
                match parsed with
                | Some value -> value
                | None -> new FormatException "Can't recognize expression" |> raise
    end
