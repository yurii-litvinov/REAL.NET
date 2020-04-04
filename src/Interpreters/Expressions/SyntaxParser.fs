module Interpreters.Expressions.SyntaxParser

open System.Collections.Immutable
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

let private convertBinToAST op =
    match op with
    | PlusOp -> AST.Plus
    | MinusOp -> AST.Minus
    

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
                        | _ -> (stack, output)
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
    
        fromPostfixToTree' lexemes []             
                        
        
let parseLexems lexemes =
    Helper.toPostfix lexemes
    