module Interpreters.Tests.SyntaxParsersTests

open FsUnit
open NUnit.Framework

open Interpreters
open Interpreters.Expressions
open Interpreters.Expressions.Lexemes
open Interpreters.Expressions.SyntaxParser

[<Test>]
let ``number of arguments test``() =
    let lexemes = [ OpeningRoundBracket; FunctionName("f"); OpeningRoundBracket; ArrayName("arr"); OpeningSquareBracket; IntConst(5); ClosingSquareBracket; ClosingRoundBracket;
                    Comma; IntConst(1); ClosingRoundBracket ]
    let number = Helper.calculateNumberOfArgs lexemes
    number |> should equal 2
    let lexemes2 = [ OpeningRoundBracket; IntConst(2); ClosingRoundBracket ]
    let number2 = Helper.calculateNumberOfArgs lexemes2
    number2 |> should equal 1

[<Test>]
let ``arythmetic expression to postfix test``() =
    let lexemes = [ OpeningRoundBracket; IntConst(10); BinOp PlusOp; IntConst(20); ClosingRoundBracket;
                   BinOp MultiplyOp;  IntConst(2) ]
    lexemes |> Helper.toPostfixExtended |> should equal
                                       [ IntConst(10); IntConst(20); BinOp PlusOp; IntConst(2); BinOp MultiplyOp ]
    
[<Test>]
let ``arythmetic expression with variables to postfix test``() =
    let lexemes = [ VariableName("a"); BinOp MultiplyOp; DoubleConst(10.0); BinOp DivideOp; DoubleConst(2.0) ]
    lexemes |> Helper.toPostfixExtended |> should equal
                                       [ VariableName("a"); DoubleConst(10.0); BinOp MultiplyOp; DoubleConst(2.0); BinOp DivideOp ]

[<Test>]                                       
let ``arythmetic expression with function to postfix test``() =
    let lexemes = [ FunctionName("func"); OpeningRoundBracket; VariableName("a"); Comma; VariableName("b"); ClosingRoundBracket ]
    let postfix = lexemes |> Helper.toPostfixExtended
    postfix |> should equal [ VariableName("a"); VariableName("b"); ExtraInfo(NumberOfArgs(2)); FunctionName("func") ]
    let lexemes2 = [ FunctionName("f") ; OpeningRoundBracket; VariableName("a"); Comma; DoubleConst(10.0);
                     ClosingRoundBracket; BinOp PlusOp; IntConst(10) ]
    let postfix2 = lexemes2 |> Helper.toPostfixExtended
    postfix2 |> should equal [ VariableName("a"); DoubleConst(10.0); ExtraInfo(NumberOfArgs(2)); FunctionName("f"); IntConst(10); BinOp PlusOp ]
    
[<Test>]
let ``logical expressions to postfix test``() =
    let lexems = [ UnOp Not; VariableName("a"); BinOp EqualityOp; VariableName("b"); BinOp InequalityOp; BoolConst(true) ]
    lexems |> Helper.toPostfixExtended |> should equal [VariableName("a"); UnOp Not; VariableName("b"); BinOp EqualityOp; BoolConst(true); BinOp InequalityOp ]
    
[<Test>]
let ``function in function to postfix test``() =
    let lexemes = [ FunctionName("func"); OpeningRoundBracket; FunctionName("f") ; OpeningRoundBracket; ClosingRoundBracket;
                     Comma; IntConst(10); ClosingRoundBracket ]
    let postfix = lexemes |> Helper.toPostfixExtended
    postfix |> should equal [ ExtraInfo(NumberOfArgs(0)); FunctionName("f"); IntConst(10); ExtraInfo(NumberOfArgs(2)); FunctionName("func") ]
    
[<Test>]
let ``array index test``() =
    let lexemes = [ ArrayName("arr"); OpeningSquareBracket; IntConst(10); BinOp PlusOp; IntConst(20); ClosingSquareBracket ]
    let postfix = lexemes |> Helper.toPostfixExtended
    postfix |> should equal [ IntConst(10); IntConst(20); BinOp PlusOp; ArrayName("arr") ]
    
[<Test>]
let ``assigment to postfix test``() =
    let lexemes = [ VariableName("a"); BinOp AssigmentOp; IntConst(10); BinOp PlusOp; IntConst(20) ]
    let postfix = lexemes |> Helper.toPostfixExtended
    postfix |> should equal [ VariableName("a"); IntConst(10); IntConst(20); BinOp PlusOp; BinOp AssigmentOp ]
    let lexemes = [ VariableName("a"); BinOp AssigmentOp; VariableName("b"); BinOp AssigmentOp; IntConst(20) ]
    (fun () -> lexemes |> Helper.toPostfixExtended |> ignore) |> should (throwWithMessage "Assigment is more than one time") typeof<SyntaxParserException>
    
[<Test>]
let ``array declaration to postfix test``() =
    let lexemes = [ NewOperator; TypeSelection PrimitiveTypes.Int; OpeningSquareBracket; IntConst(2); ClosingSquareBracket; OpeningCurlyBracket; IntConst(1); Comma; IntConst(3); ClosingCurlyBracket ]
    let postfix = lexemes |> Helper.toPostfixExtended 
    postfix |> should equal [ IntConst(2); IntConst(1); IntConst(3); ExtraInfo(NumberOfArgs 2); TypeSelection PrimitiveTypes.Int; NewOperator ]
    
[<Test>]
let ``extended postfix test``() =
    let lexemes = [ FunctionName("f"); OpeningRoundBracket; IntConst(1); ClosingRoundBracket ]
    let postfix = lexemes |> Helper.toPostfixExtended
    postfix |> should equal [ IntConst(1); ExtraInfo(NumberOfArgs(1)); FunctionName("f") ]
    
[<Test>]
let ``array declaration test``() =
    let lexemes = [ NewOperator; TypeSelection PrimitiveTypes.Int; OpeningSquareBracket; IntConst(2); BinOp PlusOp; IntConst(4); ClosingSquareBracket; OpeningCurlyBracket; IntConst(0); Comma; IntConst(1); ClosingCurlyBracket ]
    let postfix = lexemes |> Helper.toPostfixExtended
    postfix |> should equal [ IntConst(2); IntConst(4); BinOp PlusOp; IntConst(0); IntConst(1); ExtraInfo(NumberOfArgs(2)); TypeSelection PrimitiveTypes.Int; NewOperator ]
    
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
    let tree = lexemes |> SyntaxParser.parseLexemes
    tree |> should equal (Plus(
                             Function("f", [ Variable("a"); ConstOfDouble(10.0) ]),
                             ConstOfInt(10)))

[<Test>]
let ``tree with logical expressions test``() =
    let lexems = [ UnOp Not; VariableName("a"); BinOp EqualityOp; VariableName("b"); BinOp InequalityOp; BoolConst(true) ]
    let tree = lexems |> SyntaxParser.parseLexemes
    tree |> should equal (Inequality(
                                        Equality(LogicalNot(Variable("a")), Variable("b")),
                                        ConstOfBool(true)         
                                    ))

[<Test>]
let ``tree with function test``() =
    let lexemes = [ FunctionName("func"); OpeningRoundBracket; ClosingRoundBracket ]
    let tree = lexemes |> SyntaxParser.parseLexemes
    tree |> should equal (Function("func", []))
    let lexemes2 = [ FunctionName("func"); OpeningRoundBracket; VariableName("a"); ClosingRoundBracket ]
    let tree2 = lexemes2 |> SyntaxParser.parseLexemes
    tree2 |> should equal (Function("func", [ Variable("a") ]))
    let lexemes3 = [ FunctionName("func"); OpeningRoundBracket; FunctionName("f"); OpeningRoundBracket; VariableName("a"); ClosingRoundBracket; Comma
                     FunctionName("g"); OpeningRoundBracket; VariableName("b"); ClosingRoundBracket; ClosingRoundBracket ]
    let tree3 = lexemes3 |> SyntaxParser.parseLexemes
    tree3 |> should equal (Function("func", [ Function("f", [ Variable("a") ]); Function("g", [ Variable("b") ]) ]))
    
[<Test>]
let ``array declaration to tree test``() =
    let lexemes = [ VariableName("a"); BinOp AssigmentOp; NewOperator; TypeSelection PrimitiveTypes.Int; OpeningSquareBracket; IntConst(2); ClosingSquareBracket;
                    OpeningCurlyBracket; IntConst(0); Comma; IntConst(1); ClosingCurlyBracket ]
    let tree = SyntaxParser.parseLexemes lexemes
    tree |> should equal (Assigment(Variable("a"),
                                    ArrayDeclaration(PrimitiveTypes.Int, ConstOfInt(2), [ ConstOfInt(0); ConstOfInt(1) ])   
                                   ))