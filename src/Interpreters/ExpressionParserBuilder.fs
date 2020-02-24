namespace Interpreters.Expressions

open System.Linq.Expressions
open System.Text
open Interpreters.Expressions.ExpressionParsers

type StringExpressionParserBuilder private
    (
        expressionParser: StringExpressionParser
    ) =
    class
        new() = new StringExpressionParserBuilder(new StringExpressionParser())
        
        member this.Build = expressionParser :> IStringExpressionParser        
    end

