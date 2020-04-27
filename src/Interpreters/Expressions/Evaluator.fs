module Interpreters.Expressions.Evaluator

open System
open Interpreters
open Interpreters
open Interpreters.Expressions.AST

let private plus a b =
    match (a, b) with
    | (RegularValue(Int x), RegularValue(Int y)) -> x + y |> Int |> RegularValue 
    | (RegularValue(Double x), RegularValue(Double y)) -> x + y |> Double |> RegularValue 
    | (RegularValue(String x), RegularValue(String y)) -> x + y |> String |> RegularValue 
    | _ -> "Plus is not supported for this types" |> TypeException |> raise
    
let private minus a b =
    match (a, b) with
    | (RegularValue(Int x), RegularValue(Int y)) -> x - y |> Int |> RegularValue 
    | (RegularValue(Double x), RegularValue(Double y)) -> x - y |> Double |> RegularValue 
    | _ -> "Minus is not supported for this types" |> TypeException |> raise

let private mult a b =
    match (a, b) with
    | (RegularValue(Int x), RegularValue(Int y)) -> x * y |> Int |> RegularValue 
    | (RegularValue(Double x), RegularValue(Double y)) -> x * y |> Double |> RegularValue 
    | _ -> "Multiply is not supported for this types" |> TypeException |> raise
    
let private divide a b =
    try
        match (a, b) with
        | (RegularValue(Int x), RegularValue(Int y)) -> x / y |> Int |> RegularValue 
        | (RegularValue(Double x), RegularValue(Double y)) -> x / y |> Double |> RegularValue 
        | _ -> "Divide is not supported for this types" |> TypeException |> raise
    with :? DivideByZeroException -> "Divide by zero" |> EvaluatorException |> raise
    
let private equal a b = a = b |> Bool |> RegularValue

let private notEqual a b = a <> b |> Bool |> RegularValue

let private bigger a b =
    match (a, b) with
    | (RegularValue(Int x), RegularValue(Int y)) -> x > y |> Bool |> RegularValue 
    | (RegularValue(Double x), RegularValue(Double y)) -> x > y |> Bool |> RegularValue 
    | _ -> "Can't compare this types" |> TypeException |> raise
    
let private biggerOrEqual a b =
    match (a, b) with
    | (RegularValue(Int x), RegularValue(Int y)) -> x >= y |> Bool |> RegularValue 
    | (RegularValue(Double x), RegularValue(Double y)) -> x >= y |> Bool |> RegularValue 
    | _ -> "Can't compare this types" |> TypeException |> raise
    
let private less a b =
    match (a, b) with
    | (RegularValue(Int x), RegularValue(Int y)) -> x < y |> Bool |> RegularValue 
    | (RegularValue(Double x), RegularValue(Double y)) -> x < y |> Bool |> RegularValue 
    | _ -> "Can't compare this types" |> TypeException |> raise
    
let private lessOrEqual a b =
    match (a, b) with
    | (RegularValue(Int x), RegularValue(Int y)) -> x <= y |> Bool |> RegularValue 
    | (RegularValue(Double x), RegularValue(Double y)) -> x <= y |> Bool |> RegularValue 
    | _ -> "Can't compare this types" |> TypeException |> raise

let private not' v =
    match v with
    | RegularValue(Bool b) -> not b |> Bool |> RegularValue
    | _ -> "Not used for not boolean type" |> TypeException |> raise
    
let private negative a =
    match a with
    | RegularValue ar ->
        match ar with
        | Int x -> (-x) |> Int |> RegularValue
        | Double x -> (-x) |> Double |> RegularValue
        | _ -> "Not int or double to negate" |> TypeException |> raise
    | _ -> "This type could not be negate" |> TypeException |> raise
    
let private and' a b =
    match (a, b) with
    | (RegularValue(Bool x), RegularValue(Bool y)) -> (x && y) |> Bool |> RegularValue
    | _ -> "And used for not boolean type" |> TypeException |> raise
    
let private or' a b =
    match (a, b) with
    | (RegularValue(Bool x), RegularValue(Bool y)) -> (x || y) |> Bool |> RegularValue
    | _ -> "And used for not boolean type" |> TypeException |> raise
    
let private arrDecl ``type`` size init = 
    match size with
    | RegularValue (Int 0) ->
        match ``type`` with
        | PrimitiveTypes.Int -> ExpressionValue.initArr 0 (ExpressionValue.createInt 1)
        | PrimitiveTypes.Double -> ExpressionValue.initArr 0 (ExpressionValue.createDouble 1.0)
        | PrimitiveTypes.Bool -> ExpressionValue.initArr 0 (ExpressionValue.createBoolean true)
        | PrimitiveTypes.String -> ExpressionValue.initArr 0 (ExpressionValue.createString "")
        | _ -> failwith "Unknown type"
    | RegularValue (Int 1) ->
        match init with
        | [ RegularValue _ as i ] ->
            if (ExpressionValue.getType i = (PrimitiveCase ``type``)) then ExpressionValue.initArr 1 i
            else "Type of init is not correct" |> TypeException |> raise
        | _ -> "Type is not regular or init values are not correct" |> EvaluatorException |> raise
    | RegularValue (Int n) ->
        match init with
        | [ RegularValue _ as i ] ->
            if (ExpressionValue.getType i = (PrimitiveCase ``type``)) then ExpressionValue.initArr n i
            else "Type is not regular or init values are not correct" |> EvaluatorException |> raise
        | list when (List.length list = n) ->
            let arr = ExpressionValue.tryCreateArr list
            match arr with
            | Some a -> a
            | None -> "Not correct init values" |> EvaluatorException |> raise
        | _ -> "Count of init elements is not correct" |> EvaluatorException |> raise
    | _ -> "Size is not int" |> TypeException |> raise
    
let rec private function' funcList args state =
    match funcList with
    | (argsType, _, f) :: _ when argsType = (List.map (fun x -> ExpressionValue.getType x) args) ->
        f args state 
    | _ :: t -> function' t args state
    | [] -> "Can't find function with this type or count of arguments" |> EvaluatorException |> raise
    
let evaluate (env: EnvironmentOfExpressions) (node: AbstractSyntaxNode) =
    let wrap a = (env, a)
    let unwrap (env, a) = a
    let rec evaluate' node =
        match node with
        | ConstOfInt v -> ExpressionValue.createInt v |> wrap
        | ConstOfBool v -> ExpressionValue.createBoolean v |> wrap
        | ConstOfDouble v -> ExpressionValue.createDouble v |> wrap
        | ConstOfString v -> ExpressionValue.createString v |> wrap
        | Variable(name) ->
            let var = env.Variables.TryFindByName(name)
            match var with
            | Some x -> x.Value |> wrap
            | None -> "Can't find variable with name " + name |> EvaluatorException |> raise
        | Plus(a, b) -> plus (evaluate' a |> unwrap) (evaluate' b |> unwrap) |> wrap
        | Minus(a, b) -> minus (evaluate' a |> unwrap) (evaluate' b |> unwrap) |> wrap
        | Multiply(a, b) -> mult (evaluate' a |> unwrap) (evaluate' b |> unwrap) |> wrap
        | Divide(a, b) -> divide (evaluate' a |> unwrap) (evaluate' b |> unwrap) |> wrap
        | ArrayDeclaration(``type``, size, init) ->
            let s = (evaluate' size |> unwrap)
            let initVals = List.map (fun a -> evaluate' a |> unwrap) init
            arrDecl ``type`` s initVals |> wrap
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
            | IndexAt(arrName, index) ->
                let i = (evaluate' index |> unwrap)
                let v = (evaluate' value |> unwrap)
                match i with
                | RegularValue (Int n) ->
                    match env.Variables.TryFindByName arrName with
                    | Some arr ->
                            match arr.Value with
                            | ArrayValue a ->
                                if (ExpressionValue.getType v = (PrimitiveCase (ArrType.getElementType a))) then
                                    let newArrOption = ExpressionValue.tryChangeValueAtIndex n v arr.Value
                                    match newArrOption with
                                    | Some newArr ->
                                        let newEnv = env.ChangeValue arr newArr
                                        (newEnv, Void)
                                    | None -> 
                                        "Array index assigment error: index out of range exception"
                                        |> EvaluatorException |> raise
                                else "Array type and type of value to assign are mismatched" |> TypeException |> raise
                            | _ -> "Not array" |> EvaluatorException |> raise
                    | None -> "Can't find array with this name" |> EvaluatorException |> raise
                | _ -> "Index of array is not int" |> TypeException |> raise
            | _ -> failwith "Unexpected error: left part type"
        | Equality(a, b) -> (equal (evaluate' a |> unwrap) (evaluate' b |> unwrap)) |> wrap
        | Inequality(a, b) -> (notEqual (evaluate' a |> unwrap) (evaluate' b |> unwrap)) |> wrap
        | Bigger(a, b) -> (bigger (evaluate' a |> unwrap) (evaluate' b |> unwrap)) |> wrap
        | BiggerOrEqual(a, b) -> (biggerOrEqual (evaluate' a |> unwrap) (evaluate' b |> unwrap)) |> wrap
        | Less(a, b) -> (less (evaluate' a |> unwrap) (evaluate' b |> unwrap)) |> wrap
        | LessOrEqual(a, b) -> (lessOrEqual (evaluate' a |> unwrap) (evaluate' b |> unwrap)) |> wrap
        | LogicalNot v -> (not' (evaluate' v |> unwrap)) |> wrap
        | LogicalAnd(a, b) -> (and' (evaluate' a |> unwrap) (evaluate' b |> unwrap)) |> wrap
        | LogicalOr(a, b) -> (or' (evaluate' a |> unwrap) (evaluate' b |> unwrap)) |> wrap
        | Negative a -> negative (evaluate' a |> unwrap) |> wrap
        | IndexAt(arrName, index) ->
            let iOption = (evaluate' index |> unwrap)
            match iOption with
            | RegularValue (Int n) ->
                match env.Variables.TryFindByName arrName with
                | Some arr ->
                    if ExpressionValue.isArray arr.Value then
                        match ExpressionValue.tryGetValueAtIndex n arr.Value with
                        | Some a -> (env, a)
                        | _ -> "Index out of range" |> EvaluatorException |> raise
                    else "It is not array" |> TypeException |> raise
                | None -> "Can't find array with this name" |> EvaluatorException |> raise
            | _ -> "Index is not int" |> TypeException |> raise
        | Function(funcName, args) ->
            match env.Functions.TryFind funcName with
            | Some funcList -> 
                let (result, newState) = function' funcList (List.map (fun x -> (evaluate' x) |> unwrap) args) env.State
                let newEnv = env.NewState(newState)
                (newEnv, result)
            | None -> "Can't find function with this name" |> EvaluatorException |> raise
        | Temp _ -> failwith "Unexpected error: temp data in evaluator"    
         
    evaluate' node
