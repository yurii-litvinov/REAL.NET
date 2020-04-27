module Interpreters.Tests.StringToValueTests

open NUnit.Framework
open FsUnit

open Interpreters
open Interpreters.Expressions

let initEnv =
    let sum list s =
        match list with
        | [ RegularValue (Int a); RegularValue (Int b) ] ->
            (a + b |> Int |> RegularValue, s)
        | _ -> "Not correct types of args" |> TypeException |> raise
    let sumExt = ([ PrimitiveCase PrimitiveTypes.Int; PrimitiveCase PrimitiveTypes.Int ], PrimitiveCase PrimitiveTypes.Int, sum)
    let sumFunc = ("sum", [ sumExt ])
    let funcs = Map([ sumFunc ])
    let aVar = Variable.createInt "a" 1 (None, None)
    let bVar = Variable.createBoolean "b" true (None, None)
    let arr = Variable.createVar "arr" (ArrayValue (ArrType.tryCreateArray([ Int 1; Int 2; Int 3 ]).Value)) (None, None)
    let vars = VariableSet.VariableSetFactory.CreateVariableSet([ aVar; bVar; arr ])
    let state = StateConsole.empty
    EnvironmentOfExpressions(vars, funcs, state)

let eval expr = 
    let env = initEnv
    expr |> Lexer.parseString |> SyntaxParser.parseLexemes |> Evaluator.evaluate env
    
[<Test>]
let ``simple arythmetic test``() =
    let expression = "(a + 2) * arr[2]"
    let value = ExpressionValue.createInt 9
    expression |> eval |> should equal (initEnv, value)
    