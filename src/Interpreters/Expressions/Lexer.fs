module Interpreters.Expressions.Lexer

open Interpreters
open System.Text.RegularExpressions
    
let eatMatching matching (input: string) =
    if input.StartsWith(matching) then input.Substring(matching.Length)
    else (invalidArg "matching" "It is not beginning of input")
    
module StringPatterns =
    let private createCompiledRegex pattern =
        new Regex(pattern, RegexOptions.Compiled)
    
    let private returnMatchOptionWrapper (regex: Regex) input =
        let m = regex.Match(input)
        if m.Success then m.Value |> Some
        else None
        
    let private intRegex = "^-?\d+" |> createCompiledRegex
    
    let private doubleRegex = "^-?\d+\.\d+" |> createCompiledRegex
        
    let private nameRegex = "^_?[a-z|A-Z](\w|_)*" |> createCompiledRegex
        
    let private stringRegex = "^\"(\\.{1}|[^\"])*\"" |> createCompiledRegex 
        
    let private boolRegex = "^(true|false)" |> createCompiledRegex
    
    let private typeRegex = "^(int|double|bool|string)" |> createCompiledRegex
    
    let (|IntToken|_|) input =
        let matched = returnMatchOptionWrapper intRegex input
        match matched with
        | Some x -> Some(int x |> IntConst, eatMatching x input)
        | _ -> None
        
    let (|DoubleToken|_|) input =
        let matched = returnMatchOptionWrapper doubleRegex input
        match matched with
        | Some x -> Some(double x |> DoubleConst, eatMatching x input)
        | _ -> None
    
    let (|NameToken|_|) input =
        let matched = returnMatchOptionWrapper nameRegex input
        match matched with
        | Some x -> Some(x |> Name, eatMatching x input)
        | _ -> None
    
    let (|StringToken|_|) input =
        let matched = returnMatchOptionWrapper stringRegex input
        match matched with
        | Some x -> Some(x |> StringConst, eatMatching x input)
        | _ -> None
    
    let (|BoolToken|_|) input =
        let matched = returnMatchOptionWrapper boolRegex input
        match matched with
        | Some x -> Some((TypeTransform.tryBool x).Value |> BoolConst, eatMatching x input)
        | _ -> None
    
    let (|TypeToken|_|) input =
        let matched = returnMatchOptionWrapper typeRegex input
        match matched with
        | Some xType -> match xType with
                        | "int" -> Some(PrimitiveTypes.Int |> TypeSelection, eatMatching "int" input)
                        | "double" -> Some(PrimitiveTypes.Double |> TypeSelection, eatMatching "double" input)
                        | "bool" -> Some(PrimitiveTypes.Bool |> TypeSelection, eatMatching "bool" input)
                        | "string" -> Some(PrimitiveTypes.String |> TypeSelection, eatMatching "string" input)
                        | ``type`` -> failwith ("unknown type" + ``type``)
        | _ -> None
        
    let (|NewToken|_|) (input: string) =
        if (input.Length >=3) && input.[0..2] = "new" then Some(NewOperator, eatMatching "new" input) else None
        
    let (|BinOpToken|_|) (input: string) =
        let (|OneSymbolOp|_|) (input: string) =
            if (input.Length >= 1) then
                match input.[0] with
                | '+' -> Some(PlusOp, eatMatching "+" input)
                | '-' -> Some(MinusOp, eatMatching "-" input)
                | '*' -> Some(MultiplyOp, eatMatching "*" input)
                | '/' -> Some(DivideOp, eatMatching "/" input)
                | '=' -> Some(AssigmentOp, eatMatching "=" input)
                | _ -> None
            else None
        let (|TwoSymbolsOp|_|) (input: string) =
            if (input.Length >= 2) then
                match input.[0..1] with
                | "==" -> Some(EqualityOp, eatMatching "==" input)
                | "!=" -> Some(InequalityOp, eatMatching "!=" input)
                | _ -> None
            else None
        match input with
        | TwoSymbolsOp matching -> Some(matching)
        | OneSymbolOp matching -> Some(matching)
        | _ -> None
        
    let (|UnOperatorToken|_|) (input: string) =
        if (input.Length >= 2) then
            match input.[0] with
            | '!' -> Some(Not, eatMatching "!" input)
            | '-' when (input.[1] <> ' ') -> Some(Negative, eatMatching "-" input)
            | _ -> None
        else None
        
    let (|BracketToken|_|) (input: string) =
        if (input.Length >= 1) then
            match input.[0] with
            | '(' -> Some(OpeningRoundBracket, eatMatching "(" input)
            | ')' -> Some(ClosingRoundBracket, eatMatching ")" input)
            | '[' -> Some(OpeningSquareBracket, eatMatching "[" input)
            | ']' -> Some(ClosingSquareBracket, eatMatching "]" input)
            | '{' -> Some(OpeningCurlyBracket, eatMatching "{" input)
            | '}' -> Some(ClosingCurlyBracket, eatMatching "}" input)
            | _ -> None
        else None
    
    let (|CommaToken|_|) (input: string) =
        if (input.Length >= 1) && input.[0] = ',' then Some(Comma, eatMatching "," input) else None
        
let parseString (input: string) =
    let rec parse lexems (input: string) =
        None
    parse [] input
            