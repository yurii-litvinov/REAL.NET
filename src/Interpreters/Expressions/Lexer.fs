module Interpreters.Expressions.Lexer

module CharacterHelper =
    let isLetter c = (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z')

    let isDigit c = (c >= '0' && c <= '9')
    
    let isUnderscore c = c = '_'

open System.Text.RegularExpressions
open System

let (|Regex|_|) pattern input =
        let m = Regex.Match(input, pattern)
        if m.Success then Some(m.Value)
        else None

let (|FirstRegex|_|) pattern input =
    match input with
    | Regex pattern matching -> if input.StartsWith(matching) then Some matching
                                else None
    | _ -> None
    
let eatMatching matching (input: string) =
    if input.StartsWith(matching) then input.Substring(matching.Length)
    else (invalidArg "matching" "It is not beginning of input")
    
module StringPatterns =
    let intPattern = "-?\d+"
    
    let doublePattern = "-?\d+\.\d+"
    
    let namePattern = "_?[a-z|A-Z](\w)*"
    
    let stringPattern = "\".*\""
    
    let boolPattern = "true|false"
    
    let newPattern = "new"
    
    let typePattern ="int|double|bool|string"
    
module StringToTokenConverter =
    let x = 0
    
let parseString (input: string) =
    let rec parse lexems (input: string) =
        None
    parse [] input
            