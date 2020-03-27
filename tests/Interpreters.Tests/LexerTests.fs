module Interpreters.Tests.LexerTests

open FsUnit
open NUnit.Framework

open Interpreters.Expressions.Lexer
open NUnit.Framework.Internal

[<Test>]
let ``int validation test``() =
    let pattern = StringPatterns.intPattern
    let firstString = "123 456"
    match firstString with
    | FirstRegex pattern matching -> matching |> should equal "123"
    | _ -> Assert.Fail "No matching first string"
    let secondString = "-456 789"
    match secondString with
    | FirstRegex pattern matching -> matching |> should equal "-456"
    | _ -> Assert.Fail "No matching second string"
    let thirdString = "x123"
    match thirdString with
    | FirstRegex pattern _ -> Assert.Fail "Should not be matched"
    | _ -> ignore 0

[<Test>]
let ``double validation test``() = 
    let pattern = StringPatterns.doublePattern
    let firstString = "123.001 456"
    match firstString with
    | FirstRegex pattern matching -> matching |> should equal "123.001"
    | _ -> Assert.Fail "No matching in first string"
    let secondString = "-123.456 a"
    match secondString with
    | FirstRegex pattern matching -> matching |> should equal "-123.456"
    | _ -> Assert.Fail "No matching in second string"
    let thirdString = "124 456"
    match thirdString with
    | FirstRegex pattern m -> Assert.Fail <| "Should not be matched" + m.ToString()
    | _ -> ignore 0

[<Test>]
let ``bool validation test``() =
    let pattern = StringPatterns.boolPattern
    let trueString = "true"
    match trueString with
    | FirstRegex pattern matching -> matching |> should equal "true"
    | _ -> Assert.Fail "No matching in true string"
    let falseString = "false"
    match falseString with
    | FirstRegex pattern matching -> matching |> should equal "false"
    | _ -> Assert.Fail "No matching with false string"
    let someString = "MyString"
    match someString with
    | FirstRegex pattern _ -> Assert.Fail "Should not be matched"
    | _ -> ignore 0
    
[<Test>]
let ``name validation test``() =
    let pattern = StringPatterns.namePattern
    let firstString = "var1 + var2"
    match firstString with
    | FirstRegex pattern matching -> matching |> should equal "var1"
    | _ -> Assert.Fail "No matching with first string"
    let secondString = "_myVariable1ef / 1234"
    match secondString with
    | FirstRegex pattern matching -> matching |> should equal "_myVariable1ef"
    | _ -> Assert.Fail "No matching with second string"
    let thirdString = "_123"
    match thirdString with
    | FirstRegex pattern _ -> Assert.Fail "Should not be matched"
    | _ -> ignore 0
  
[<Test>]
let ``string validation test``() =
    let pattern = StringPatterns.stringPattern
    let quote = "\""
    let firstString = quote + "String" + quote + "Some other info"
    match firstString with
    | FirstRegex pattern matching -> matching |> should equal (quote + "String" + quote)
    | _ -> Assert.Fail "No matching in first string"
    let secondString = quote + "String"
    match secondString with
    | FirstRegex pattern _ -> Assert.Fail "Should not be matched"
    | _ -> ignore 0
    
[<Test>]
let ``new operator pattern test``() =
    let pattern = StringPatterns.newPattern
    let newString = "new"
    match newString with
    | FirstRegex pattern matching -> matching |> should equal "new"
    | _ -> Assert.Fail "No matching in new operator"
    let notStrString = "notStr"
    match notStrString with
    | FirstRegex pattern _ -> Assert.Fail "Should not be matched"
    | _ -> ignore 0
    
[<Test>]
let ``type pattern test``() =
    let pattern = StringPatterns.typePattern
    let intString = "int"
    match intString with
    | FirstRegex pattern matching -> matching |> should equal "int"
    | _ -> Assert.Fail "No int type matching"
    let doubleString = "double"
    match doubleString with
    | FirstRegex pattern matching -> matching |> should equal "double"
    | _ -> Assert.Fail "No double type matching"
    let boolString = "bool"
    match boolString with
    | FirstRegex pattern matching -> matching |> should equal "bool"
    | _ -> Assert.Fail "No bool type matching"
    let stringString = "string"
    match stringString with
    | FirstRegex pattern matching -> matching |> should equal "string"
    | _ -> Assert.Fail "No string type matching"
    let someString = "someType"
    match someString with
    | FirstRegex pattern matching -> Assert.Fail "Should not be matched"
    | _ -> ignore 0
    
    
    
    
    
