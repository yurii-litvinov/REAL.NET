module Interpreters.Tests.SyntaxParsersTests

open FsUnit
open NUnit.Framework

open Interpreters.Expressions
open Interpreters.Expressions.Lexemes
open Interpreters.Expressions.SyntaxParser

[<Test>]
let ``arythmetic expression to postfix test``() =
    let lexemes = [ OpeningRoundBracket; IntConst(10); BinOp PlusOp; IntConst(20); ClosingRoundBracket;
                   BinOp MultiplyOp;  IntConst(2) ]
    lexemes |> Helper.toPostfix |> should equal
                                       [ IntConst(10); IntConst(20); BinOp PlusOp; IntConst(2); BinOp MultiplyOp ]
    
[<Test>]
let ``arythmetic expression with variables to postfix test``() =
    let lexemes = [ VariableName("a"); BinOp MultiplyOp; DoubleConst(10.0); BinOp DivideOp; DoubleConst(2.0) ]
    lexemes |> Helper.toPostfix |> should equal
                                       [ VariableName("a"); DoubleConst(10.0); BinOp MultiplyOp; DoubleConst(2.0); BinOp DivideOp ]

[<Test>]                                       
let ``arythmetic expression with function to postfix test``() =
    let lexemes = [ FunctionName("func"); OpeningRoundBracket; VariableName("a"); Comma; VariableName("b"); ClosingRoundBracket ]
    lexemes |> Helper.toPostfix |> should equal [ VariableName("a"); VariableName("b"); FunctionName("func") ]
    let lexemes2 = [ FunctionName("f") ; OpeningRoundBracket; VariableName("a"); Comma; DoubleConst(10.0);
                     ClosingRoundBracket; BinOp PlusOp; IntConst(10) ]
    let postfix = lexemes2 |> Helper.toPostfix
    postfix |> should equal [ VariableName("a"); DoubleConst(10.0); FunctionName("f"); IntConst(10); BinOp PlusOp ]
    
open AST

[<Test>]
let ``arythmetic syntax tree builder test``() =
    let lexemes = [ OpeningRoundBracket; IntConst(10); BinOp PlusOp; IntConst(20); ClosingRoundBracket;
                   BinOp MultiplyOp;  IntConst(2) ]
    lexemes |> SyntaxParser.parseLexemes
    |> should equal (Multiply(
                             Plus(ConstOfInt(10), ConstOfInt(20)),
                             ConstOfInt(2))) 

[<Test>]
let ``arythmetic syntax tree builder with functions and variables test``() =
    let lexemes = [ FunctionName("f") ; OpeningRoundBracket; VariableName("a"); Comma; DoubleConst(10.0);
                     ClosingRoundBracket; BinOp PlusOp; IntConst(10) ]
    let postfix = lexemes |> Helper.toPostfix
    let tree = postfix |> Helper.fromPostfixToTree
    tree |> should equal (Plus(
                             Function("f", [ Variable("a"); ConstOfDouble(10.0) ]),
                             ConstOfInt(10)))
    