module LogoTests

open NUnit
open NUnit.Framework
open FsUnit

[<Test>]
let tr() = true |> should be True

