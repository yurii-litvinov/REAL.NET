module LogoParserTests

open NUnit
open NUnit.Framework
open FsUnit

open Repo
open Interpreters.VariableSet
open Interpreters.Parser
open Interpreters
open Languages.Logo.LogoParser
open Languages.Logo.TurtleCommand


let repo = RepoFactory.Create()

let metamodel = repo.Model "LogoMetamodel"

let model = repo.Model "LogoModel"

let isInitial (element: IElement) = element.Class.Name = "InitialNode"

let isFinal (element: IElement) = element.Class.Name = "FinalNode"

let initialNode = model.Elements |> Seq.filter (fun e -> e.Class.Name = "InitialNode") |> Seq.exactlyOne

let finalNode = model.Elements |> Seq.filter (fun e -> e.Class.Name = "FinalNode") |> Seq.exactlyOne

let findAllEdgesFrom (element: IElement) =
    model.Edges |> Seq.filter (fun (e: IEdge) -> e.From = element) 

let findAllEdgesTo (element: IElement) =
    model.Edges |> Seq.filter (fun (e: IEdge) -> e.To = element) 

let next (element: IElement) = let edge = findAllEdgesFrom element |> Seq.exactlyOne in edge.To

let firstForward = next initialNode

let firstRight = next firstForward

let secondForward = next firstRight

let emtyVariableSet = VariableSetFactory.CreateVariableSet([])

[<Test>]
let ``forward should be parsed correctly``() = 
    let context = {Commands = []; Model = model}
    let (parsing: Parsing<Context>) = (emtyVariableSet, context, firstForward)
    let wrapped = Some parsing
    AvailibleParsers.parseForward wrapped |> should not' (equal None)
    let parsed = (AvailibleParsers.parseForward wrapped).Value
    Parsing.element parsed |> should be (equal firstRight)
    let newContext = Parsing.context parsed
    let command = newContext.Commands.Head
    command |> should be (equal (LForward 100.0))

[<Test>]
let ``right should be parsed correctly``() = 
    let context = {Commands = [LForward 100.0]; Model = model}
    let (parsing: Parsing<Context>) = (emtyVariableSet, context, firstRight)
    let wrapped = Some parsing
    AvailibleParsers.parseRight wrapped |> should not' (equal None)
    let parsed = (AvailibleParsers.parseRight wrapped).Value
    Parsing.element parsed |> should be (equal secondForward)
    let newContext = Parsing.context parsed
    let command = newContext.Commands.Head
    command |> should be (equal (LRight 90.0))  

[<Test>]
let ``complex movement parsing``() =
    let context = {Commands = []; Model = model}
    let (parsing: Parsing<Context>) = (emtyVariableSet, context, firstForward)
    let wrapped = Some parsing
    let parsedOnce = (parseMovement wrapped).Value
    let parsedTwice = (parseMovement (Some parsedOnce)).Value
    let newElement = Parsing.element parsedTwice 
    newElement |> should equal secondForward
    let newContext = Parsing.context parsedTwice
    newContext.Commands |> should be (equal [LRight 90.0; LForward 100.0])




