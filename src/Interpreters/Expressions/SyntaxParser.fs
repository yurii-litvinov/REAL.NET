module Interpreters.Expressions.SyntaxParser

open Interpreters.Expressions
open System.Collections.Immutable
open System.Reflection
open Interpreters.Expressions.Lexemes

let prior op =
    match op with
    | PlusOp -> 6
    | MinusOp -> 6
    | DivideOp -> 7
    | MultiplyOp -> 7
    | EqualityOp -> 4
    | InequalityOp -> 4
    | BiggerOp -> 4
    | LessOp -> 4
    | BiggerOrEqualOp -> 4
    | LessOrEqualOp -> 4
    | AndOp -> 3
    | OrOp -> 2
    | AssigmentOp -> 0

let private getConstructor op =
    match op with
    | PlusOp -> AST.Plus
    | MinusOp -> AST.Minus
    | DivideOp -> AST.Divide
    | MultiplyOp -> AST.Multiply
    | AndOp -> AST.LogicalAnd
    | OrOp -> AST.LogicalOr
    | EqualityOp -> AST.Equality
    | InequalityOp -> AST.Inequality
    | _ -> failwith "Not implemented"

module Helper =
    let toPostfix lexemes = 
        let rec toPostfix' lexemes stack (output: ImmutableQueue<_>) =
            match lexemes with
            | lexeme :: tail ->
                match lexeme with
                | IntConst _ as c -> toPostfix' tail stack (output.Enqueue(c)) 
                | DoubleConst _ as c -> toPostfix' tail stack (output.Enqueue(c)) 
                | BoolConst _ as c -> toPostfix' tail stack (output.Enqueue(c)) 
                | StringConst _ as c -> toPostfix' tail stack (output.Enqueue(c)) 
                | VariableName _ as n -> toPostfix' tail stack (output.Enqueue(n))
                | FunctionName _ as n -> toPostfix' tail (n :: stack) output
                | Comma ->
                    let rec toOutput stack (output: ImmutableQueue<_>) =
                        match stack with
                        | h :: t when h = OpeningRoundBracket -> (h :: t, output)
                        | h :: t -> toOutput t (output.Enqueue(h))
                        | _ -> "There is no opening bracket or comma" |> SyntaxParserException |> raise
                    let (newStack, newOutput) = toOutput stack output
                    toPostfix' tail newStack newOutput
                | BinOp op1 ->
                    let rec toOutput stack (output: ImmutableQueue<_>) =
                        match stack with
                        | BinOp op2 :: t when (prior op2) >= (prior op1) -> toOutput t (output.Enqueue(BinOp op2))
                        | FunctionName _ as f :: t -> toOutput t (output.Enqueue(f))
                        | _ -> (stack, output) // TODO unary operator
                    let (newStack, newOutput) = toOutput stack output
                    toPostfix' tail (BinOp op1 :: newStack) newOutput
                | OpeningRoundBracket as b -> toPostfix' tail (b :: stack) output
                | ClosingRoundBracket ->
                    let rec toOutput stack (output: ImmutableQueue<_>) =
                        match stack with
                        | OpeningRoundBracket :: t -> (t, output)
                        | h :: t -> toOutput t (output.Enqueue(h))
                        | _ -> "There is no opening bracket" |> SyntaxParserException |> raise
                    let (newStack, newOutput) = toOutput stack output
                    toPostfix' tail newStack newOutput
                | _ -> failwith "Not implemented yet"
            | _ ->
                let rec toOutput stack (output: ImmutableQueue<_>) =
                    match stack with
                    | h :: _ when h = OpeningRoundBracket || h = ClosingRoundBracket -> "Missed bracket" |> SyntaxParserException |> raise
                    | h :: t -> toOutput t (output.Enqueue(h))
                    | _ -> output
                let queue = toOutput stack output
                let toList (queue: ImmutableQueue<_>) =
                    let rec toList' (queue: ImmutableQueue<_>) list =
                        if queue.IsEmpty then list
                        else let element = queue.Peek()
                             toList' (queue.Dequeue()) (element :: list)
                    toList' queue [] |> List.rev
                queue |> toList
        toPostfix' lexemes [] ImmutableQueue.Empty
        
    let fromPostfixToTree lexemes =
        let rec fromPostfixToTree' lexemes stack =
            match lexemes with
            | lexeme :: tail ->
                match lexeme with
                | IntConst x -> fromPostfixToTree' tail (AST.ConstOfInt x :: stack)
                | DoubleConst x -> fromPostfixToTree' tail (AST.ConstOfDouble x :: stack)
                | BoolConst x -> fromPostfixToTree' tail (AST.ConstOfBool x :: stack)
                | StringConst x -> fromPostfixToTree' tail (AST.ConstOfString x :: stack)
                | VariableName x -> fromPostfixToTree' tail (AST.Variable x :: stack)
                | BinOp AssigmentOp ->
                    match stack with
                    | right :: left :: stackEnd ->
                        match left with
                        | AST.Variable _ as var -> fromPostfixToTree' tail (AST.Assigment(var, right) :: stackEnd)
                        | _ -> "Left part of assigment is not variable" |> SyntaxParserException |> raise
                    | _ -> "No operands for assigment" |> SyntaxParserException |> raise
                | FunctionName func ->
                    let funcNode = AST.Function(func, List.rev stack)
                    fromPostfixToTree' tail [funcNode]
                | ArrayName arr ->
                    match stack with
                    | index :: stackEnd -> fromPostfixToTree' tail (AST.IndexAt(arr, index) :: stackEnd)
                    | _ -> "No index is found" |> SyntaxParserException |> raise
                | BinOp op ->
                    match stack with
                    | right :: left :: stackEnd -> fromPostfixToTree' tail (getConstructor op (left, right) :: stackEnd)
                    | _ -> "No operands for " + op.ToString() |> SyntaxParserException |> raise
            | _ ->
                match stack with
                | [ node ] -> node
                | _ -> "Can't recognize" |> SyntaxParserException |> raise
        fromPostfixToTree' lexemes []             
    
        
let parseLexemes lexemes =
    let postfix =  Helper.toPostfix lexemes
    postfix |> Helper.fromPostfixToTree
    