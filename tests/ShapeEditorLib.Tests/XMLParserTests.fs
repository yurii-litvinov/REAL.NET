module XMLParserTests

open NUnit.Framework
open FsUnit
open ShapeEditorLib.Parsers
open System.IO
open System

let parser = XMLParser()

let init() =
    let path = TestContext.CurrentContext.TestDirectory
    parser.LoadFile(Path.Combine(path, "Files/TestView.xml"))

init()

[<Test>]
let ``attribute should be parsed``() =
    let attributeView = parser.ParseAttributeProperties("Attribute1")
    attributeView.AttributeName |> should equal "Attribute1"
    attributeView.ExampleValue |> should equal "ExampleValue1"
    attributeView.IsVisible |> should be True
    attributeView.OrderNumber |> should equal 1
    attributeView.Position.Value|> should equal <| (100, 200).ToValueTuple()

