module LogoParserTests

open NUnit.Framework
open FsUnit

open Repo
open Interpreters
open Interpreters.VariableSet
open Interpreters.Logo.LogoParser
open Interpreters.Logo.Tests
open Interpreters.Logo.TurtleCommand

let repo = RepoFactory.Create()

let metamodelName = "LogoMetamodel"

open Helper

let initModel() =
    let model = getModel()
    let initialNode = createInitialNode()
    let finalNode = createFinalNode()

    let forwards = [ for _ in [1..4] -> createForward "100.0" ]

    let rights = [ for _ in [1..4] -> createRight "90.0" ]
    
    let left = createLeft "90.0"

    let backward = createBackward "100.0"
    
    let repeat = createRepeat "2"

    initialNode --> 
    repeat --> 
    forwards.[0] --> rights.[0] --> forwards.[1] --> rights.[1] --> forwards.[2] --> rights.[2] --> 
    forwards.[3] --> rights.[3] --> 
    left --> backward --> repeat
    |> ignore

    let exit = repeat +-> finalNode
    do setAttribute exit "Tag" "Exit"
    model

let model =
    resetModel()
    initModel()

let elements = List.ofSeq model.Elements

let initialNode = model.Elements |> Seq.filter (fun e -> e.Class.Name = "InitialNode") |> Seq.exactlyOne

let finalNode = model.Elements |> Seq.filter (fun e -> e.Class.Name = "FinalNode") |> Seq.exactlyOne

let isInitial (element: IElement) = element.Class.Name = "InitialNode"

let isFinal (element: IElement) = element.Class.Name = "FinalNode"

let findAllEdgesFrom (element: IElement) = ElementHelper.outgoingEdges model element

let findAllEdgesTo (element: IElement) = ElementHelper.incomingEdges model element

let hasAttribute name (element: IElement) = ElementHelper.hasAttribute name element

let next (element: IElement) =
    let allEdges = element |> ElementHelper.outgoingEdges model
    let edge = allEdges |> Seq.filter (hasAttribute "Tag" >> not) |> Seq.exactlyOne
    edge.To

let repeat = next initialNode

let firstForward = next repeat

let firstRight = next firstForward

let secondForward = next firstRight

let emtyVariableSet = VariableSetFactory.CreateVariableSet([])

[<Test>]
let ``forward should be parsed correctly``() = 
    let context = {Commands = []}
    let (parsing: Parsing<Context>) = { Variables = emtyVariableSet; Context = context; Model = model; Element = firstForward }
    let wrapped = Some parsing
    AvailableParsers.parseForward wrapped |> should not' (equal None)
    let parsed = (AvailableParsers.parseForward wrapped).Value
    Parsing.element parsed |> should be (equal firstRight)
    let newContext = Parsing.context parsed
    let command = newContext.Commands.Head
    command |> should be (equal (LForward 100.0))

[<Test>]
let ``right should be parsed correctly``() = 
    let context = {Commands = [LForward 100.0]}
    let (parsing: Parsing<Context>) = { Variables = emtyVariableSet; Context = context; Model = model; Element = firstRight }
    let wrapped = Some parsing
    AvailableParsers.parseRight wrapped |> should not' (equal None)
    let parsed = (AvailableParsers.parseRight wrapped).Value
    Parsing.element parsed |> should be (equal secondForward)
    let newContext = Parsing.context parsed
    let command = newContext.Commands.Head
    command |> should be (equal (LRight 90.0))  

[<Test>]
let ``complex movement parsing``() =
    let context = {Commands = []}
    let (parsing: Parsing<Context>) = { Variables = emtyVariableSet; Context = context; Model = model; Element = firstForward}
    let wrapped = Some parsing
    let parsedOnce = (parseMovement wrapped).Value
    let parsedTwice = (parseMovement (Some parsedOnce)).Value
    let newElement = Parsing.element parsedTwice 
    newElement |> should equal secondForward
    let newContext = Parsing.context parsedTwice
    newContext.Commands |> should be (equal [LRight 90.0; LForward 100.0])




