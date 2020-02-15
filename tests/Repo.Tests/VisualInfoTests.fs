module Repo.Tests.VisualInfoTests

open NUnit.Framework
open FsUnit

open Repo
open Repo.Visual

// VisualPoint tests
[<Test>]
let ``construct should create correct point``() =
    let point = new VisualPoint(100.0, 200.0)
    point.X |> should be (equal 100.0)
    point.Y |> should be (equal 200.0)

[<Test>]
let ``default should return zero point``() = VisualPoint.Default |> should be (equal (new VisualPoint(0.0, 0.0)))

// VisualNodeInfo tests
let getNodeinfo() = new VisualNodeInfo() :> IVisualNodeInfo

[<Test>]
let ``position should be correct``() =
    let info = getNodeinfo()
    info.Position <- new VisualPoint(1.0, 2.0)
    info.Position |> should be (equal (new VisualPoint(1.0, 2.0)))
    