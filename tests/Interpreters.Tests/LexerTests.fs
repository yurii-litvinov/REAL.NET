module Interpreters.Tests.LexerTests

open FsUnit
open NUnit.Framework

open Interpreters
open Interpreters.Expressions.Lexer
open Interpreters.Expressions
open StringPatterns

[<Test>]
let ``int validation test``() =
    let firstString = "123 456"
    match firstString with
    | IntToken matching -> matching |> should equal (IntConst 123, " 456")    
    | _ -> Assert.Fail "No matching first string"
    let secondString = "-456 789"
    match secondString with
    | IntToken matching -> matching |> should equal (IntConst -456, " 789")
    | _ -> Assert.Fail "No matching second string"
    let thirdString = "x123"
    match thirdString with
    | IntToken _ -> Assert.Fail "Should not be matched"
    | _ -> ignore 0

[<Test>]
let ``double validation test``() = 
    let firstString = "123.001 456"
    match firstString with
    | DoubleToken matching -> matching |> should equal (DoubleConst 123.001, " 456")
    | _ -> Assert.Fail "No matching in first string"
    let secondString = "-123.456 a"
    match secondString with
    | DoubleToken matching -> matching |> should equal (DoubleConst -123.456, " a")
    | _ -> Assert.Fail "No matching in second string"
    let thirdString = "124 456"
    match thirdString with
    | DoubleToken m -> Assert.Fail <| "Should not be matched" + m.ToString()
    | _ -> ignore 0

[<Test>]
let ``bool validation test``() =
    let trueString = "true 123"
    match trueString with
    | BoolToken matching -> matching |> should equal (BoolConst true, " 123")
    | _ -> Assert.Fail "No matching in true string"
    let falseString = "false 456"
    match falseString with
    | BoolToken matching -> matching |> should equal (BoolConst false, " 456")
    | _ -> Assert.Fail "No matching with false string"
    let someString = "MyString false"
    match someString with
    | BoolToken _ -> Assert.Fail "Should not be matched"
    | _ -> ignore 0
    
[<Test>]
let ``name validation test``() =
    let firstString = "var1 + var2"
    match firstString with
    | NameToken matching -> matching |> should equal (Name "var1", " + var2")
    | _ -> Assert.Fail "No matching with first string"
    let secondString = "_myVariable1ef / 1234"
    match secondString with
    | NameToken matching -> matching |> should equal (Name "_myVariable1ef", " / 1234")
    | _ -> Assert.Fail "No matching with second string"
    let thirdString = "_123"
    match thirdString with
    | NameToken _ -> Assert.Fail "Should not be matched"
    | _ -> ignore 0
  
[<Test>]
let ``string validation test``() =
    let quote = "\""
    let firstString = quote + "String" + quote + " Some \"other\" info"
    match firstString with
    | StringToken matching -> matching |> should equal (StringConst (quote + "String" + quote), " Some \"other\" info")
    | _ -> Assert.Fail "No matching in first string"
    let secondString = quote + "String"
    match secondString with
    | StringToken _ -> Assert.Fail "Should not be matched"
    | _ -> ignore 0
    
[<Test>]
let ``type pattern test``() =
    let intString = "int a"
    match intString with
    | TypeToken matching -> matching |> should equal (TypeSelection PrimitiveTypes.Int, " a")
    | _ -> Assert.Fail "No int type matching"
    let doubleString = "double b"
    match doubleString with
    | TypeToken matching -> matching |> should equal (TypeSelection PrimitiveTypes.Double, " b")
    | _ -> Assert.Fail "No double type matching"
    let boolString = "bool c"
    match boolString with
    | TypeToken matching -> matching |> should equal (TypeSelection PrimitiveTypes.Bool, " c")
    | _ -> Assert.Fail "No bool type matching"
    let stringString = "string d"
    match stringString with
    | TypeToken matching -> matching |> should equal (TypeSelection PrimitiveTypes.String, " d")
    | _ -> Assert.Fail "No string type matching"
    let someString = "someType"
    match someString with
    | TypeToken _ -> Assert.Fail "Should not be matched"
    | _ -> ignore 0
    
[<Test>]
let ``new operator pattern test``() =
    let newString = "new int"
    match newString with
    | NewToken matching -> matching |> should equal (NewOperator, " int")
    | _ -> Assert.Fail "No matching in new operator"
    let notStrString = "notStr"
    match notStrString with
    | NewToken _ -> Assert.Fail "Should not be matched"
    | _ -> ignore 0
    
[<Test>]
let ``bin operators validation test``() =
    let plusString = "+ -some"
    match plusString with
    | BinOpToken matching -> matching |> should equal (PlusOp, " -some")
    | _ -> Assert.Fail "No matching plus"
    let minusString = "- + some"
    match minusString with
    | BinOpToken matching -> matching |> should equal (MinusOp, " + some")
    | _ -> Assert.Fail "No matching minus"
    let multiplyString = "* some"
    match multiplyString with
    | BinOpToken matching -> matching |> should equal (MultiplyOp, " some")
    | _ -> Assert.Fail "No matching multiply"
    let divideString = "/ some"
    match divideString with
    | BinOpToken matching -> matching |> should equal (DivideOp, " some")
    | _ -> Assert.Fail "No matching divide"
    let equalityString = "== some"
    match equalityString with
    | BinOpToken matching -> matching |> should equal (EqualityOp, " some")
    | _ -> Assert.Fail "No matching equality"
    let inequalityString = "!= some"
    match inequalityString with
    | BinOpToken matching -> matching |> should equal (InequalityOp, " some")
    | _ -> Assert.Fail "No matching inequality"

[<Test>]    
let ``unary operator validation test``() =
    let negativeString = "-a + b"
    match negativeString with
    | UnOperatorToken matching -> matching |> should equal (Negative, "a + b")
    | _ -> Assert.Fail "No matching negative operator"
    let notString = "!ab cd"
    match notString with
    | UnOperatorToken matching -> matching |> should equal (Not, "ab cd")
    | _ -> Assert.Fail "No matching logical not operator"
    
[<Test>]
let ``brackets validation test``() =
    let openRoundString = "(test"
    match openRoundString with
    | BracketToken matching -> matching |> should equal (OpeningRoundBracket, "test")
    | _ -> Assert.Fail "No matching opening round bracket"
    let closeRoundString = ")tested some code"
    match closeRoundString with
    | BracketToken matching -> matching |> should equal (ClosingRoundBracket, "tested some code")
    | _ -> Assert.Fail "No matching closing round bracket"
    let openSquareString = "[test"
    match openSquareString with
    | BracketToken matching -> matching |> should equal (OpeningSquareBracket, "test")
    | _ -> Assert.Fail "No matching opening square bracket"
    let closeSquareString = "]tested some code"
    match closeSquareString with
    | BracketToken matching -> matching |> should equal (ClosingSquareBracket, "tested some code")
    | _ -> Assert.Fail "No matching closing square bracket"
    let openCurlyString = "{test"
    match openCurlyString with
    | BracketToken matching -> matching |> should equal (OpeningCurlyBracket, "test")
    | _ -> Assert.Fail "No matching opening curly bracket"
    let closeCurlyString = "}tested some code"
    match closeCurlyString with
    | BracketToken matching -> matching |> should equal (ClosingCurlyBracket, "tested some code")
    | _ -> Assert.Fail "No matching closing curly bracket"
    
let ``comma validation test``() =
    let commaString = ", some test"
    match commaString with
    | CommaToken matching -> matching |> should equal (Comma, " some test")
    | _ -> Assert.Fail "No matching comma"