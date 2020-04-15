module Interpreters.Expressions.Evaluator

open System
open Interpreters
open Interpreters.Expressions.AST

let private plus a b =
    match (a, b) with
    | (RegularValue(Int x), RegularValue(Int y)) -> x + y |> Int |> RegularValue 
    | (RegularValue(Double x), RegularValue(Double y)) -> x + y |> Double |> RegularValue 
    | (RegularValue(String x), RegularValue(String y)) -> x + y |> String |> RegularValue 
    | _ -> "Plus is not supported for this types" |> invalidOp
    
let private minus a b =
    match (a, b) with
    | (RegularValue(Int x), RegularValue(Int y)) -> x - y |> Int |> RegularValue 
    | (RegularValue(Double x), RegularValue(Double y)) -> x - y |> Double |> RegularValue 
    | _ -> "Minus is not supported for this types" |> invalidOp

let private mult a b =
    match (a, b) with
    | (RegularValue(Int x), RegularValue(Int y)) -> x * y |> Int |> RegularValue 
    | (RegularValue(Double x), RegularValue(Double y)) -> x * y |> Double |> RegularValue 
    | _ -> "Multiply is not supported for this types" |> invalidOp
    
let private divide a b =
    try
        match (a, b) with
        | (RegularValue(Int x), RegularValue(Int y)) -> x / y |> Int |> RegularValue 
        | (RegularValue(Double x), RegularValue(Double y)) -> x / y |> Double |> RegularValue 
        | _ -> "Divide is not supported for this types" |> invalidOp
    with :? DivideByZeroException -> "Divide by zero" |> EvaluatorException |> raise
    
let private equal a b = a = b |> Bool |> RegularValue

let private notEqual a b = a <> b |> Bool |> RegularValue

let private not' v =
    match v with
    | RegularValue(Bool b) -> not b |> Bool |> RegularValue
    | _ -> "Not used for not boolean type" |> TypeException |> raise
    
let private and' a b =
    match (a, b) with
    | (RegularValue(Bool x), RegularValue(Bool y)) -> (x && y) |> Bool |> RegularValue
    | _ -> "And used for not boolean type" |> TypeException |> raise
    
let private or' a b =
    match (a, b) with
    | (RegularValue(Bool x), RegularValue(Bool y)) -> (x || y) |> Bool |> RegularValue
    | _ -> "And used for not boolean type" |> TypeException |> raise
    
let private arrDecl ``type`` size init = 
    NotImplementedException() |> raise

let evaluate (env: EnvironmentOfExpressions) (node: AbstractSyntaxNode) =
    let wrap a = (env, a)
    let unwrap (env, a) = a
    let rec evaluate' node =
        match node with
        | ConstOfInt v -> VariableValue.createInt v |> wrap
        | ConstOfBool v -> VariableValue.createBoolean v |> wrap
        | ConstOfDouble v -> VariableValue.createDouble v |> wrap
        | ConstOfString v -> VariableValue.createString v |> wrap
        | Variable(name) as v ->
            let var = env.Variables.TryFindByName(name)
            match var with
            | Some x -> x.Value |> wrap
            | None -> "Can't find variable with name " + name |> EvaluatorException |> raise
        | Plus(a, b) -> plus (evaluate' a |> unwrap) (evaluate' b |> unwrap) |> wrap
        | Minus(a, b) -> minus (evaluate' a |> unwrap) (evaluate' b |> unwrap) |> wrap
        | Multiply(a, b) -> mult (evaluate' a |> unwrap) (evaluate' b |> unwrap) |> wrap
        | Divide(a, b) -> divide (evaluate' a |> unwrap) (evaluate' b |> unwrap) |> wrap
        | ArrayDeclaration(``type``, size, init) -> arrDecl ``type`` size init |> raise
        | Assigment(x, value) ->
            match x with
            | Variable varName ->
                let varOption = env.Variables.TryFindByName varName
                match varOption with
                | Some a -> let right = (evaluate' value |> unwrap)
                            (env.ChangeValue a right, Void)
                | None -> let right = (evaluate' value |> unwrap)
                          let v = Variable.createVar varName right (env.Place |> PlaceOfCreation.extract)
                          (env.AddVariable v, Void)
            | IndexAt _ -> NotImplementedException() |> raise
            | _ -> failwith "Unexpected error: left part type"
        | Equality(a, b) -> (equal (evaluate' a |> unwrap) (evaluate' b |> unwrap)) |> wrap
        | Inequality(a, b) -> (notEqual (evaluate' a |> unwrap) (evaluate' b |> unwrap)) |> wrap
        | LogicalNot v -> (not' (evaluate' v |> unwrap)) |> wrap
        | LogicalAnd(a, b) -> (and' (evaluate' a |> unwrap) (evaluate' b |> unwrap)) |> wrap
        | LogicalOr(a, b) -> (or' (evaluate' a |> unwrap) (evaluate' b |> unwrap)) |> wrap
    evaluate' node
