module Interpreters.Expressions.SyntaxParser

open Interpreters.Expressions
open Interpreters.Expressions.ExpressionParsers
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
    
let priorUn op =
    match op with
    | NegativeOp -> 9
    | Not -> 9

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

let private getUnOpConstructor op =
    match op with
    | Not -> AST.LogicalNot
    | NegativeOp -> AST.Negative
    
module Helper =
    let calculateNumberOfArgs lexemes =
        let rec calculateNumberOfArgs' lexemes round square curly count =
            if (round < 0 && square < 0 && curly < 0) then "Mismatched brackets" |> SyntaxParserException |> raise
            elif (round + square + curly) = 0 then count + 1
            else match lexemes with
                 | OpeningRoundBracket :: tail  -> calculateNumberOfArgs' tail (round + 1) square curly count
                 | ClosingRoundBracket :: tail  -> calculateNumberOfArgs' tail (round - 1) square curly count
                 | OpeningSquareBracket :: tail  -> calculateNumberOfArgs' tail round (square + 1) curly count
                 | ClosingSquareBracket :: tail  -> calculateNumberOfArgs' tail round (square - 1) curly count
                 | OpeningCurlyBracket :: tail -> calculateNumberOfArgs' tail round square (curly + 1) count
                 | ClosingCurlyBracket :: tail -> calculateNumberOfArgs' tail round square (curly - 1) count
                 | Comma :: tail -> if (round + square + curly) = 1 then calculateNumberOfArgs' tail round square curly (count + 1)
                                                                    else calculateNumberOfArgs' tail round square curly count
                 | _ :: tail -> calculateNumberOfArgs' tail round square curly count
                 | [] -> if (round + square + curly) = 0 then count + 1
                         else "Mismatched brackets" |> SyntaxParserException |> raise
        match lexemes with
        | OpeningRoundBracket :: ClosingRoundBracket :: _ -> 0
        | OpeningSquareBracket :: ClosingSquareBracket :: _ -> 0
        | OpeningCurlyBracket :: ClosingCurlyBracket :: _ -> 0
        | OpeningRoundBracket :: tail -> calculateNumberOfArgs' tail 1 0 0 0
        | OpeningSquareBracket :: tail -> calculateNumberOfArgs' tail 0 1 0 0
        | OpeningCurlyBracket :: tail -> calculateNumberOfArgs' tail 0 0 1 0
        | _ -> failwith "Unexpected first lexeme"
    
    let rec insertNumberOfArgs lexemes =
        let rec insertNumberOfArgs' lexemes output =
            match lexemes with
            | FunctionName _ as n :: tail ->
                let info = calculateNumberOfArgs tail |> NumberOfArgs |> ExtraInfo
                insertNumberOfArgs' tail (info :: n :: output)
            | NewOperator :: (TypeSelection _ as t) :: tail ->
                let info = calculateNumberOfArgs tail |> NumberOfArgs |> ExtraInfo
                insertNumberOfArgs' tail (info :: t :: NewOperator :: output)
            | h :: t -> insertNumberOfArgs' t (h :: output)
            | [] -> List.rev output
        insertNumberOfArgs' lexemes []
    
    let toPostfixExtended lexemes =
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
                | ExtraInfo _ as i -> toPostfix' tail (i :: stack) output 
                | ArrayName _ as n -> toPostfix' tail (n :: stack) output
                | Comma ->
                    let rec toOutput stack (output: ImmutableQueue<_>) =
                        match stack with
                        | h :: t when h = OpeningRoundBracket -> (h :: t, output)
                        | h :: t when h = OpeningCurlyBracket -> (h :: t, output)
                        | h :: t -> toOutput t (output.Enqueue(h))
                        | _ -> "There is no opening bracket or comma" |> SyntaxParserException |> raise
                    let (newStack, newOutput) = toOutput stack output
                    toPostfix' tail newStack newOutput
                | UnOp op1 ->
                    let rec toOutput stack (output: ImmutableQueue<_>) =
                        match stack with
                        | UnOp op2 :: t when (priorUn op2) >= (priorUn op1) -> toOutput t (output.Enqueue(UnOp op2))
                        | (ExtraInfo(NumberOfArgs _) as i) :: (FunctionName _ as f) :: t -> toOutput t (output.Enqueue(i).Enqueue(f))
                        | _ -> (stack, output)
                    let (newStack, newOutput) = toOutput stack output
                    toPostfix' tail (UnOp op1 :: newStack) newOutput
                | BinOp op1 ->
                    let rec toOutput stack (output: ImmutableQueue<_>) =
                        match stack with
                        | BinOp op2 :: t when (prior op2) >= (prior op1) -> toOutput t (output.Enqueue(BinOp op2))
                        | (ExtraInfo(NumberOfArgs _) as i) :: (FunctionName _ as f) :: t -> toOutput t (output.Enqueue(i).Enqueue(f))
                        | UnOp _ as unOp :: t -> toOutput t (output.Enqueue(unOp))
                        | _ -> (stack, output) 
                    let (newStack, newOutput) = toOutput stack output
                    toPostfix' tail (BinOp op1 :: newStack) newOutput
                | OpeningRoundBracket as b -> toPostfix' tail (b :: stack) output
                | ClosingRoundBracket ->
                    let rec toOutput stack (output: ImmutableQueue<_>) =
                        match stack with
                        | OpeningRoundBracket :: t -> (t, output)
                        | h :: t -> toOutput t (output.Enqueue(h))
                        | _ -> "There is no opening round bracket" |> SyntaxParserException |> raise
                    let (newStack, newOutput) = toOutput stack output
                    toPostfix' tail newStack newOutput
                | OpeningSquareBracket as b -> toPostfix' tail (b :: stack) output
                | ClosingSquareBracket ->
                    let rec toOutput stack (output: ImmutableQueue<_>) =
                        match stack with
                        | OpeningSquareBracket :: t -> (t, output)
                        | h :: t -> toOutput t (output.Enqueue(h))
                        | _ -> "There is no opening square bracket" |> SyntaxParserException |> raise
                    let (newStack, newOutput) = toOutput stack output
                    toPostfix' tail newStack newOutput
                | NewOperator as newOp -> toPostfix' tail (newOp :: stack) output
                | TypeSelection _ as sel -> toPostfix' tail (sel :: stack) output
                | OpeningCurlyBracket as b -> toPostfix' tail (b :: stack) output
                | ClosingCurlyBracket ->
                    let rec toOutput stack (output: ImmutableQueue<_>) =
                        match stack with
                        | OpeningCurlyBracket :: t -> (t, output)
                        | h :: t -> toOutput t (output.Enqueue(h))
                        | _ -> "There is no opening curly bracket" |> SyntaxParserException |> raise
                    let (newStack, newOutput) = toOutput stack output
                    toPostfix' tail newStack newOutput
                
            | _ ->
                let rec toOutput stack (output: ImmutableQueue<_>) =
                    match stack with
                    | h :: _ when h = OpeningRoundBracket || h = ClosingRoundBracket -> "Missed round bracket" |> SyntaxParserException |> raise
                    | h :: _ when h = OpeningSquareBracket || h = ClosingSquareBracket -> "Missed square bracket" |> SyntaxParserException |> raise
                    | h :: _ when h = OpeningCurlyBracket || h = ClosingCurlyBracket -> "Missed curly bracket" |> SyntaxParserException |> raise
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
                
        let extended = lexemes |> insertNumberOfArgs
        let assigments = extended |> List.indexed |> List.filter (fun (_, el) -> el = BinOp AssigmentOp)
        match assigments with
        | [] -> toPostfix' extended [] ImmutableQueue.Empty
        | [ (index, el) ] -> let (l1, l2) = List.splitAt index lexemes
                             let postfix1 = toPostfix' l1 [] ImmutableQueue.Empty
                             let postfix2 = toPostfix' (List.tail l2) [] ImmutableQueue.Empty
                             postfix1 @ postfix2 @ [ el ]
        | _ -> "Assigment is more than one time" |> SyntaxParserException |> raise
        
    let toPostfix lexemes =
        let isInfo x =
            match x with
            | ExtraInfo _ -> true
            | _ -> false
        lexemes |> toPostfixExtended |> List.filter (isInfo >> not)         
        
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
                        | AST.IndexAt _ as index -> fromPostfixToTree' tail (AST.Assigment(index, right) :: stackEnd)
                        // NOTICE left part another type?
                        | _ -> "Left part of assigment is not variable or array index" |> SyntaxParserException |> raise
                    | _ -> "No operands for assigment" |> SyntaxParserException |> raise
                | FunctionName func ->
                    let funcNode = AST.Function(func, List.rev stack)
                    fromPostfixToTree' tail [funcNode]
                | ArrayName arr ->
                    match stack with
                    | index :: stackEnd -> fromPostfixToTree' tail (AST.IndexAt(arr, index) :: stackEnd)
                    | _ -> "No index is found" |> SyntaxParserException |> raise
                | UnOp op ->
                    match stack with
                    | operand :: stackEnd -> fromPostfixToTree' tail (getUnOpConstructor op operand :: stackEnd)
                    | _ -> "No operand for " + op.ToString() |> SyntaxParserException |> raise
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
    