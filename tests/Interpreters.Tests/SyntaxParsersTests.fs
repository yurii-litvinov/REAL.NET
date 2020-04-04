module Interpreters.Tests.SyntaxParsersTests

open FsUnit
open NUnit.Framework

open Interpreters.Expressions.Lexemes
open Interpreters.Expressions.SyntaxParser

[<Test>]
let ``arythmetic expression to postfix test``() =
    let lexemes = [ OpeningRoundBracket; IntConst(10); BinOp PlusOp; IntConst(20); ClosingRoundBracket;
                   BinOp MultiplyOp;  IntConst(2) ]
    lexemes |> Helper.toPostfix |> should equivalent
                                       [ IntConst(10); IntConst(20); BinOp PlusOp; IntConst(2); BinOp MultiplyOp ]
    
[<Test>]
let ``arythmetic expression wirh variables test``() =
    let lexemes = [ VariableName("a"); BinOp MultiplyOp; DoubleConst(10.0); BinOp DivideOp; DoubleConst(2.0) ]
    lexemes |> Helper.toPostfix |> should equivalent
                                       [ VariableName("a"); DoubleConst(10.0); BinOp MultiplyOp; DoubleConst(2.0); BinOp DivideOp ]

[<Test>]                                       
let ``arythmetic expression with function test``() =
    let lexemes = [ FunctionName("func"); OpeningRoundBracket; VariableName("a"); Comma; VariableName("b"); ClosingRoundBracket ]
    lexemes |> Helper.toPostfix |> should equivalent [ VariableName("a"); VariableName("b"); FunctionName("func") ]