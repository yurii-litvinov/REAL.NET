module Interpreters.Tests.EvaluatorTests

open FsUnit
open NUnit.Framework


open Interpreters
open Interpreters.Expressions
open Interpreters.Expressions.AST

let initEnv = EnvironmentOfExpressions.getStandardEnvironment

[<Test>]
let ``simple arithmetic test``() =
    let node = Multiply(ConstOfInt(2), ConstOfInt(3))
    let value = VariableValue.createInt 6
    Evaluator.evaluate initEnv node |> should equal (initEnv, value)
    let node2 = Plus(Multiply(ConstOfDouble(2.0), ConstOfDouble(3.0)), ConstOfDouble(3.0))
    let value2 = VariableValue.createDouble 9.0
    Evaluator.evaluate initEnv node2 |> should equal (initEnv, value2)
 
[<Test>]
let ``node with variablet tests``() =
    let node = Multiply(Variable("a"), ConstOfInt(2))
    let var = Variable.createInt "a" 4 (None, None)
    let env = initEnv.AddVariable(var)
    let value = VariableValue.createInt 8
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
    let true' = VariableValue.createBoolean true
    Evaluator.evaluate env node |> should equal (env, true')
    let node2 = Inequality(Variable("a"), ConstOfInt(2))
    let node3 = Equality(Variable("a"), ConstOfInt(2))
    let false' = VariableValue.createBoolean false
    Evaluator.evaluate env node2 |> should equal (env, true')
    Evaluator.evaluate env node3 |> should equal (env, false')
    