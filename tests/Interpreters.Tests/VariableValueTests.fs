module Interpreters.Tests.VariableValueTests

open NUnit.Framework
open FsUnit

open System.Collections.Immutable
open Interpreters

// VariableValue type & module tests.
[<Test>]
let ``isTypesEqual for VariableValue should be correct``() =
    VariableValue.isTypesEqual (RegularValue (Int 1)) (RegularValue (Int 2)) |> should be True
    VariableValue.isTypesEqual (RegularValue (Int 1)) (RegularValue (Double 1.0)) |> should be False

[<Test>]
let ``isRegular should be correct``() =
    VariableValue.isRegular (RegularValue (Int 1)) |> should be True
    VariableValue.isRegular (RegularValue (Double 1.0)) |> should be True

[<Test>]
let ``create array test``() =
    let arr = [ 1; 2; 3; 4; 5 ] |> List.map (VariableValue.createInt) |> VariableValue.tryCreateArr
    match arr with
    | Some (ArrayValue x) -> x |> ArrType.toList |> should equal (List.map Int [ 1; 2; 3; 4; 5 ])
    | _ -> Assert.Fail "Not created"
    
[<Test>]
let ``init array test``() =
    let init = VariableValue.createInt 1
    let n = 5
    VariableValue.initArr n init |> should equal (ArrayValue (IntArray(ImmutableArray.CreateRange([ 1; 1; 1; 1; 1 ]))))

