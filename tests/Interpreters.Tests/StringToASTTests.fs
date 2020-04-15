module Interpreters.Tests.StringToASTTests

open FsUnit
open NUnit.Framework

open Interpreters
open Interpreters.Expressions
open Interpreters.Expressions.AST

[<Test>]
let ``arythmetic expression to AST test``() =
    let expression = "a + b * c"
    let tree = expression |> Lexer.parseString |> SyntaxParser.parseLexemes
    tree |> should equal (Plus(
                                Variable("a"),
                                Multiply(Variable("b"), Variable("c"))
                          ))
    let bracketExpression = "(a + b) * c"
    let bracketTree = bracketExpression |> Lexer.parseString |> SyntaxParser.parseLexemes
    bracketTree |> should equal (Multiply(
                                            Plus(Variable("a"), Variable("b")),
                                            Variable("c")
                                 ))
    
[<Test>]
let ``function expression to AST test``() =
    let expression = "func(f(a), arr[1])"
    let tree = expression |> Lexer.parseString |> SyntaxParser.parseLexemes
    tree |> should equal (Function("func", [ Function("f", [ Variable("a") ]); IndexAt("arr", ConstOfInt(1)) ]))
                                                                                       
    
[<Test>]
let ``array declaration to AST test``() =
    let expression = "arr = new int[2]{1, 3}"
    let tree = expression |> Lexer.parseString |> SyntaxParser.parseLexemes
    tree |> should equal (Assigment(
                                       Variable("arr"),
                                       ArrayDeclaration(PrimitiveTypes.Int, ConstOfInt(2), [ ConstOfInt(1); ConstOfInt(3) ])
                                   ))
    