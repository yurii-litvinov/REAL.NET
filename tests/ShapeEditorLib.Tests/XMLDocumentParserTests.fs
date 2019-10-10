module ShapeEditorLibTests

open System
open System.Xml
open NUnit.Framework
open FsUnit
open ShapeEditorLib.Parsers.XML
open System.IO

let initParser() =
    let testDoc = XmlDocument()
    let path = TestContext.CurrentContext.TestDirectory
    testDoc.Load(Path.Combine(path, "Files/TestView.xml"))
    XMLDocumentParser(testDoc)

let parser = initParser();

[<Test>]
let ``parser should get element's name``() =
    parser.GetName() |> should equal "TestElement"

[<Test>]
let ``parser should parse properties``() = 
     let properties = parser.GetProperties()
     properties.Count |> should equal 2
     properties.ContainsKey("Property1") |> should be True
     properties.ContainsKey("Property2") |> should be True
     properties.["Property1"] |> should equal "Value1"
     properties.["Property2"] |> should equal "Value2"
        
[<Test>]
let ``parser should find attribute names``() =
    let names = parser.GetAttributesNames()
    let nameList = Seq.toList names
    nameList |> should equal ["Attribute1"; "Attribute2"]

[<Test>]
let ``parser should get properties of attribute``() =
    let properties = parser.GetAttributeProperties("Attribute1")
    properties.Count |> should equal 4
    properties.ContainsKey("ExampleValue") |> should be True
    properties.ContainsKey("IsVisible") |> should be True
    properties.["ExampleValue"] |> should equal "ExampleValue1"
    properties.["IsVisible"] |> should equal "true"