module InstanceTest

open NUnit.Framework
open FsUnit

open QuickGraph
open System.Collections.Generic
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns

open Repo
open MetamodelOperations

let private isNode = function
    | (_, Node) -> true
    | _ -> false

let private testClass = { Id = newId(); Name = "Test Class"; Potency = 1; Level = 0; }, Node
let private testClassWithPotency2 = { Id = newId(); Name = "Test Class 2"; Potency = 2; Level = 0; }, Node
let private testAttributeWithPotency0 = { Id = newId(); Name = "TestAttribute0"; Potency = 0; Level = 0 }, NodeKind.Attribute { Value = AttributeValue.String "asdf" }
let private testAttributeWithPotency1 = { Id = newId(); Name = "TestAttribute1"; Potency = 1; Level = 0 }, NodeKind.Attribute { Value = AttributeValue.None }
let private testAttributeType = { Id = newId(); Name = "String"; Potency = 1; Level = 0; }, Node

let private abstractTestClass = { Id = MetamodelOperations.newId (); Name = "Abstract Test Class"; Potency = 0; Level = 0; }, Node
let private repo : RepoRepresentation = (new BidirectionalGraph<_, _> true, new Dictionary<_, _>())

let private createAttributeEdgeLabel () = 
    let modelElementAttributes = { Id = newId(); Name = "Attribute"; Potency = 0; Level = 0 }
    (modelElementAttributes, Attribute)

let private createTypeEdgeLabel () = 
    let modelElementAttributes = { Id = newId(); Name = "Type"; Potency = 0; Level = 0 }
    (modelElementAttributes, Type)

let initGraph () =
    let graph = fst repo
    graph.AddVertex testClass |> ignore
    graph.AddVertex testClassWithPotency2 |> ignore
    graph.AddVertex abstractTestClass |> ignore
    graph.AddVertex testAttributeWithPotency0 |> ignore
    graph.AddVertex testAttributeWithPotency1 |> ignore
    graph.AddVertex testAttributeType |> ignore
    graph.AddEdge (new TaggedEdge<_, _>(testClass, testAttributeWithPotency0, createAttributeEdgeLabel ())) |> ignore
    graph.AddEdge (new TaggedEdge<_, _>(testClass, testAttributeWithPotency1, createAttributeEdgeLabel ())) |> ignore
    graph.AddEdge (new TaggedEdge<_, _>(testAttributeWithPotency0, testAttributeType, createTypeEdgeLabel ())) |> ignore
    graph.AddEdge (new TaggedEdge<_, _>(testAttributeWithPotency1, testAttributeType, createTypeEdgeLabel ())) |> ignore

[<SetUp>]
let setUp () =
    (fst repo).Clear ()
    initGraph ()

[<Test>]
let ``Instance of a node shall return node`` () =
    let instance = MetamodelOperations.instance repo (Vertex testClass) <| Map.ofList [testAttributeWithPotency1, String "value"]
    isNode instance |> should be True

[<Test>]
let ``Instance shall be added to a repo`` () =
    let instance = MetamodelOperations.instance repo (Vertex testClass) <| Map.ofList [testAttributeWithPotency1, String "value"]
    let graph = fst repo
    graph.ContainsVertex instance |> should be True

[<Test>]
let ``Instance shall have 'instanceOf' relation with class`` () =
    let instance = MetamodelOperations.instance repo (Vertex testClass) <| Map.ofList [testAttributeWithPotency1, String "value"]
    let classes = snd repo
    classes.ContainsKey(Vertex instance) |> should be True
    classes.[Vertex instance] |> should equal (Vertex testClass)

[<Test>]
let ``Instance of a node with potency 0 should fail`` () =
    (fun () -> (MetamodelOperations.instance repo (Vertex abstractTestClass) (Map.ofList []) |> ignore)) |> should throw typeof<InstantiationOfAbstractElementException>

[<Test>]
let ``Attributes with potency 0 shall disappear`` () =
    let instance = MetamodelOperations.instance repo (Vertex testClass) <| Map.ofList [testAttributeWithPotency1, String "value"]
    effectiveAttributes repo instance |> Seq.filter (fun x -> x.name = "TestAttribute0") |> should be Empty

[<Test>]
let ``Attributes with potency 1 shall remain and have value`` () =
    let instance = MetamodelOperations.instance repo (Vertex testClass) <| Map.ofList [testAttributeWithPotency1, String "value"]
    effectiveAttributes repo instance |> Seq.filter (fun x -> x.name = "TestAttribute1") |> should not' (be Empty)
    effectiveAttributes repo instance |> Seq.filter (fun x -> x.name = "TestAttribute1") |> Seq.exactlyOne |> fun x -> x.value |> should equal "value"

[<Test>]
let ``Attributes with potency 1 shall have potency 0`` () =
    let instance = MetamodelOperations.instance repo (Vertex testClass) <| Map.ofList [testAttributeWithPotency1, String "value"]
    effectiveAttributes repo instance |> Seq.filter (fun x -> x.name = "TestAttribute1") |> should not' (be Empty)
    repo |> fst |> fun x -> x.Vertices |> Seq.filter (fun ({Id = _; Name = n; Potency = p; Level = _}, _) -> n = "TestAttribute1" && p = 0) |> should not' (be Empty)

[<Test>]
let ``Instance of element with potency 1 shall have potency 0`` () =
    let instance = MetamodelOperations.instance repo (Vertex testClass) <| Map.ofList [testAttributeWithPotency1, String "value"]
    instance |> fst |> fun x -> x.Potency |> should equal 0

[<Test>]
let ``Instance of element with potency 2 shall be 1 or 0`` () =
    let instance = MetamodelOperations.instance repo (Vertex testClassWithPotency2) <| Map.ofList [testAttributeWithPotency1, String "value"]
    let potency = instance |> fst |> fun x -> x.Potency 
    potency |> should lessThan 2
    potency  |> should greaterThanOrEqualTo 0

[<Test>]
let ``Attributes with potency 1 shall still have type when instanced`` () =
    let instance = MetamodelOperations.instance repo (Vertex testClass) <| Map.ofList [testAttributeWithPotency1, String "value"]
    let instanceAttribute = 
        (fst repo).Vertices
        |> Seq.filter (fun (_, kind) -> match kind with | NodeKind.Attribute _ -> true | _ -> false) 
        |> Seq.filter ((<>) testAttributeWithPotency0)
        |> Seq.filter ((<>) testAttributeWithPotency1)
        |> Seq.exactlyOne
        
    let outEdges = (fst repo).OutEdges instanceAttribute

    outEdges |> Seq.map (fun e -> e.Tag) |> Seq.filter (function | (_, EdgeKind.Type) -> true | _ -> false) |> should not' (be Empty)
