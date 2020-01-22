module Interpreters.Parser

open Interpreters.Common

open Repo

type Parsing<'T> = 
    IVariableSet * 'T * IElement

type Parser<'T> = Parsing<'T> option -> Parsing<'T> option

let combine (parser1: Parser<'T>) (parser2: Parser<'T>) = 
    let combine' p1 p2 parsing =
        if parsing = None then None
        else match p1 parsing with
                | None -> p2 parsing
                | x -> x
    combine' parser1 parser2
     
let (>>+) parser1 parser2 = combine parser1 parser2
    
    

