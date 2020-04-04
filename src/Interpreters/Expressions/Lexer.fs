module Interpreters.Expressions.Lexer

open Interpreters
open Interpreters.Expressions.Lexemes
open System.Text.RegularExpressions
    
let private eatMatching matching (input: string) =
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
        
    let private VariableRegex = "^_?([a-z]|[A-Z])(\w|_)*" |> createCompiledRegex
    
    let private FunctionRegex = "^_?([a-z]|[A-Z])(\w|_)*\s*\(" |> createCompiledRegex
        
    let private stringRegex = "^\"(\\.{1}|[^\"])*\"" |> createCompiledRegex 
        
    let private boolRegex = "^(true|false)[^\w_]" |> createCompiledRegex
    
    let private typeRegex = "^(int|double|bool|string)[^\w_]" |> createCompiledRegex
    
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
        let matched = returnMatchOptionWrapper FunctionRegex input
        match matched with
        | Some x -> let newX = x.Substring(x.Length - 1)
                    Some(newX |> FunctionName, eatMatching newX input)
        | _ -> let matched = returnMatchOptionWrapper VariableRegex input
               match matched with
               | Some x -> Some(x |> VariableName, eatMatching x input)
               | _ -> None
    
    let (|StringToken|_|) input =
        let matched = returnMatchOptionWrapper stringRegex input
        match matched with
        | Some x -> Some(x |> StringConst, eatMatching x input)
        | _ -> None
    
    let (|BoolToken|_|) input =
        let matched = returnMatchOptionWrapper boolRegex input
        match matched with
        | Some x -> let newX = x.Substring(0, x.Length - 1)
                    Some((TypeTransform.tryBool newX).Value |> BoolConst, eatMatching newX input)
        | _ -> None
    
    let (|TypeToken|_|) input =
        let matched = returnMatchOptionWrapper typeRegex input
        match matched with
        | Some xType -> let newXType = xType.Substring(0, xType.Length - 1)
                        match newXType with
                        | "int" -> Some(PrimitiveTypes.Int |> TypeSelection, eatMatching "int" input)
                        | "double" -> Some(PrimitiveTypes.Double |> TypeSelection, eatMatching "double" input)
                        | "bool" -> Some(PrimitiveTypes.Bool |> TypeSelection, eatMatching "bool" input)
                        | "string" -> Some(PrimitiveTypes.String |> TypeSelection, eatMatching "string" input)
                        | ``type`` -> failwith ("unknown type" + ``type``)
        | _ -> None
        
    let (|NewToken|_|) (input: string) =
        if (input.Length >=4) && input.[0..3] = "new " then Some(NewOperator, eatMatching "new" input) else None
        
    let (|BinOpToken|_|) (input: string) =
        let (|OneSymbolOp|_|) (input: string) =
            if (input.Length >= 1) then
                match input.[0] with
                | '+' -> Some(PlusOp |> BinOp, eatMatching "+" input)
                | '-' -> Some(MinusOp |> BinOp, eatMatching "-" input)
                | '*' -> Some(MultiplyOp |> BinOp, eatMatching "*" input)
                | '/' -> Some(DivideOp |> BinOp, eatMatching "/" input)
                | '=' -> Some(AssigmentOp |> BinOp, eatMatching "=" input)
                | '>' -> Some(BiggerOp |> BinOp, eatMatching ">" input)
                | '<' -> Some(LessOp |> BinOp, eatMatching "<" input)
                | _ -> None
            else None
        let (|TwoSymbolsOp|_|) (input: string) =
            if (input.Length >= 2) then
                match input.[0..1] with
                | "==" -> Some(EqualityOp |> BinOp, eatMatching "==" input)
                | "!=" -> Some(InequalityOp |> BinOp, eatMatching "!=" input)
                | ">=" -> Some(BiggerOrEqualOp |> BinOp, eatMatching ">=" input)
                | "<=" -> Some(LessOrEqualOp |> BinOp, eatMatching "<=" input)
                | "&&" -> Some(AndOp |> BinOp, eatMatching "&&" input)
                | "||" -> Some(OrOp |> BinOp, eatMatching "||" input)
                | _ -> None
            else None
        match input with
        | TwoSymbolsOp matching -> Some(matching)
        | OneSymbolOp matching -> Some(matching)
        | _ -> None
        
    let (|UnOpToken|_|) (input: string) =
        if (input.Length >= 2) then
            match input.[0] with
            | '!' -> Some(Not |> UnOp, eatMatching "!" input)
            | '-' when (input.[1] <> ' ') -> Some(Negative |> UnOp, eatMatching "-" input)
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

open StringPatterns

let private extraBlankPattern = "(/s)+"

let private blankRegex = Regex(extraBlankPattern, RegexOptions.Compiled)

let private eatExtraBlanks input =
    let oneBlankPattern = " "
    blankRegex.Replace(input, oneBlankPattern)
        
let parseString (input: string) =
    let inputWithoutExtraBlanks = eatExtraBlanks input
    let eatFirstElement (input: string) = input.[1..]
    let rec parse lexemes (input: string) =
        if (input.Length = 0) then lexemes
        elif (input.Length >= 1) && input.[0] = ' ' then eatFirstElement input |> parse lexemes
        else match input with
             | DoubleToken (lexeme, newInput) -> parse (lexeme :: lexemes) newInput
             | IntToken (lexeme, newInput) -> parse (lexeme :: lexemes) newInput
             | StringToken (lexeme, newInput) -> parse (lexeme :: lexemes) newInput
             | BoolToken (lexeme, newInput) -> parse (lexeme :: lexemes) newInput
             | TypeToken (lexeme, newInput) -> parse (lexeme :: lexemes) newInput
             | NewToken (lexeme, newInput) -> parse (lexeme :: lexemes) newInput
             | NameToken (lexeme, newInput) -> parse (lexeme :: lexemes) newInput
             | UnOpToken (lexeme, newInput) -> parse (lexeme :: lexemes) newInput
             | BinOpToken (lexeme, newInput) -> parse (lexeme :: lexemes) newInput
             | CommaToken (lexeme, newInput) -> parse (lexeme :: lexemes) newInput
             | BracketToken (lexeme, newInput) -> parse (lexeme :: lexemes) newInput
             | _ -> LexerException input |> raise
            
    parse [] inputWithoutExtraBlanks |> List.rev
    
    
            