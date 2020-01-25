namespace Interpreters

open Repo

type Parsing<'T> = 
    { Variables: IVariableSet; Context: 'T; Element: IElement }

module Parsing =

    let variables (p: Parsing<'T>) = p.Variables

    let context (p: Parsing<'T>) = p.Context

    let element (p: Parsing<'T>) = p.Element

type Parser<'T> = Parsing<'T> option -> Parsing<'T> option

module Parser =

    let combine (parser1: Parser<'T>) (parser2: Parser<'T>) = 
        let combine' (p1: Parser<'T>) p2 parsing =
            if parsing = None then None
            else match p1 parsing with
                    | None -> p2 parsing
                    | x -> x
        combine' parser1 parser2

    let (>>+) parser1 parser2 = combine parser1 parser2
     
    
    
    

