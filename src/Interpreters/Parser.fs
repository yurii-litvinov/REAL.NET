namespace Interpreters

open Repo

/// State that should be parsed.
type Parsing<'T> = 
    { Variables: IVariableSet; Context: 'T; Element: IElement }    

module Parsing =

    /// Gets the Variables component from Parsing.
    let variables (p: Parsing<'T>) = p.Variables

    /// Gets the Context component from Parsing.
    let context (p: Parsing<'T>) = p.Context

    /// Gets the element component from Parsing.
    let element (p: Parsing<'T>) = p.Element

/// Type of function which transforms one Parsing into another.
type Parser<'T> = Parsing<'T> option -> Parsing<'T> option

module Parser =

    /// Combines two parsers: tries to parse using first parser, then second.
    let combine (parser1: Parser<'T>) (parser2: Parser<'T>) = 
        let combine' (p1: Parser<'T>) p2 parsing =
            if parsing = None then None
            else match p1 parsing with
                    | None -> p2 parsing
                    | x -> x
        combine' parser1 parser2

    /// Infix variant of combine.
    let (>>+) parser1 parser2 = combine parser1 parser2
     
    
    
    

