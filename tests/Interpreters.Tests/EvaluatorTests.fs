module Interpreters.Tests.EvaluatorTests

open FsUnit
open NUnit.Framework

open Interpreters
open Interpreters.Expressions
open Interpreters.Expressions.AST

let initEnv =
    let sum list s =
        match list with
        | [ RegularValue (Int a); RegularValue (Int b) ] ->
            (a + b |> Int |> RegularValue, s)
        | _ -> "Not correct types of args" |> TypeException |> raise
    let sumExt = ([ PrimitiveCase PrimitiveTypes.Int; PrimitiveCase PrimitiveTypes.Int ], PrimitiveCase PrimitiveTypes.Int, sum)
    let sumFunc = ("sum", [ sumExt ])
    let funcs = Map([ sumFunc ])
    let vars = VariableSet.VariableSetFactory.CreateVariableSet([])
    let state = StateConsole.empty
    EnvironmentOfExpressions(vars, funcs, state)

[<Test>]
let ``simple arithmetic test``() =
    let node = Multiply(ConstOfInt(2), ConstOfInt(3))
    let value = ExpressionValue.createInt 6
    Evaluator.evaluate initEnv node |> should equal (initEnv, value)
    let node2 = Plus(Multiply(ConstOfDouble(2.0), ConstOfDouble(3.0)), ConstOfDouble(3.0))
    let value2 = ExpressionValue.createDouble 9.0
    Evaluator.evaluate initEnv node2 |> should equal (initEnv, value2)
 
[<Test>]
let ``node with variable tests``() =
    let node = Multiply(Variable("a"), ConstOfInt(2))
    let var = Variable.createInt "a" 4 (None, None)
    let env = initEnv.AddVariable(var)
    let value = ExpressionValue.createInt 8
    Evaluator.evaluate env node |> should equal (env, value)
    
[<Test>]
let ``assigment to variable test``() =
    let node = Assigment(Variable("a"), ConstOfInt(2))
    let var = Variable.createInt "a" 4 (None, None)
    let var2 = Variable.createInt "a" 2 (None, None)
    let env = initEnv.AddVariable(var)
    let env2 = initEnv.AddVariable(var2)
    Evaluator.evaluate env node |> should equal (env2, Void)
    
[<Test>]
let ``assigment to element of array test``() =
    let node = Assigment(IndexAt("arr", ConstOfInt(1)), ConstOfInt(3))
    let var = Variable.createVar "arr" (ExpressionValue.initArr 2 (ExpressionValue.createInt 1)) (None, None)
    let newVar = ([1; 3] |> List.map (Int >> RegularValue) |> ExpressionValue.tryCreateArr).Value
    let env = initEnv.AddVariable(var)
    let env2 = env.ChangeValue var newVar
    Evaluator.evaluate env node |> should equal (env2, Void)
    
[<Test>]
let ``new variable test``() =
    let node = Assigment(Variable("a"), ConstOfInt(4))
    let var = Variable.createInt "a" 4 (None, None)
    let env = initEnv.AddVariable(var)
    Evaluator.evaluate initEnv node |> should equal (env, Void)
    
[<Test>]
let ``equality and inequality tests``() =
    let var = Variable.createInt "a" 1 (None, None)
    let env = initEnv.AddVariable(var)
    let node = Equality(Variable("a"), ConstOfInt(1))
    let true' = ExpressionValue.createBoolean true
    Evaluator.evaluate env node |> should equal (env, true')
    let node2 = Inequality(Variable("a"), ConstOfInt(2))
    let node3 = Equality(Variable("a"), ConstOfInt(2))
    let false' = ExpressionValue.createBoolean false
    Evaluator.evaluate env node2 |> should equal (env, true')
    Evaluator.evaluate env node3 |> should equal (env, false')

[<Test>]
let ``compare test``() =
    let var = Variable.createInt "a" 1 (None, None)
    let env = initEnv.AddVariable(var)
    let node = BiggerOrEqual(Variable("a"), ConstOfInt(1))
    let true' = ExpressionValue.createBoolean true
    Evaluator.evaluate env node |> should equal (env, true')
    let node2 = Less(Variable("a"), ConstOfInt(2))
    let node3 = Bigger(Variable("a"), ConstOfInt(2))
    let false' = ExpressionValue.createBoolean false
    Evaluator.evaluate env node2 |> should equal (env, true')
    Evaluator.evaluate env node3 |> should equal (env, false')
    
[<Test>]
let ``negative test``() =
    let var = Variable.createInt "a" 1 (None, None)
    let value = ExpressionValue.createInt -1
    let node = Negative(Variable("a"))
    let env = initEnv.AddVariable(var)
    Evaluator.evaluate env node |> should equal (env, value)
    
[<Test>]
let ``logical test``() =
    let varA = Variable.createBoolean "a" true (None, None)
    let varB = Variable.createBoolean "b" false (None, None)
    let true' = ExpressionValue.createBoolean true
    let false' = ExpressionValue.createBoolean false
    let env = initEnv.AddVariable(varA).AddVariable(varB)
    let node = LogicalAnd(Variable("a"), Variable("b"))
    Evaluator.evaluate env node |> should equal (env, false')
    let node2 = LogicalOr(Variable("a"), Variable("b"))
    Evaluator.evaluate env node2 |> should equal (env, true')
    let node3 = LogicalNot(Variable("a"))
    Evaluator.evaluate env node3 |> should equal (env, false')
    
[<Test>]
let ``array declaration test``() =
    let node = ArrayDeclaration(PrimitiveTypes.Int, ConstOfInt(2), [ ConstOfInt(1); ConstOfInt(3) ])
    let value = ([ 1; 3 ] |> List.map (Int >> RegularValue) |> ExpressionValue.tryCreateArr).Value 
    Evaluator.evaluate initEnv node |> should equal (initEnv, value)
    let node2 = ArrayDeclaration(PrimitiveTypes.Int, ConstOfInt(2), [ ConstOfInt(3) ])
    let value2 = ([ 3; 3 ] |> List.map (Int >> RegularValue) |> ExpressionValue.tryCreateArr).Value
    Evaluator.evaluate initEnv node2 |> should equal (initEnv, value2)
    
[<Test>]
let ``get array element by index test``() =
    let value = ([ 1; 3; 2 ] |> List.map (Int >> RegularValue) |> ExpressionValue.tryCreateArr).Value
    let var = Variable.createVar "arr" value (None, None)
    let env = initEnv.AddVariable(var)
    let node = IndexAt("arr", ConstOfInt(1))
    let value = ExpressionValue.createInt 3 
    Evaluator.evaluate env node |> should equal (env, value)
    
[<Test>]
let ``function test``() =
    let var = Variable.createInt "a" 1 (None, None)
    let env = initEnv.AddVariable(var)
    let node = Function("sum", [ Variable("a"); ConstOfInt(2) ])
    let value = ExpressionValue.createInt 3
    Evaluator.evaluate env node |> should equal (env, value)
    
    
  