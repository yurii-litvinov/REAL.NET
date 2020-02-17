module Interpreters.Expressions.ExpressionParsers 

    open System
    open System.Text.RegularExpressions
    open Interpreters

    module Helper =        
        
            let parseDouble (expr: string) =
                try
                    expr |> double |> VariableValue.createDouble |> Some
                with :? FormatException -> None
            
            let parseInt (expr: string) =
                try
                    expr |> int |> VariableValue.createInt |> Some
                with :? FormatException -> None

        type StringExpressionParser =
            class
                interface IStringExpressionParser with
                    member this.Parse set stringExpression =
                        invalidOp ""
            end

  