namespace Interpreters.RobotPerformer

open System
open Repo
open Interpreters
open Interpreters.RobotPerformer.DataLayer

type IRobotContext =
    interface
        abstract member WrappedCommands: seq<RobotCommand>
        
        abstract member Robot: Robot
    end

type RobotContext(robot: Robot, commands: RCommand list) =
    let wrappedCommands = List.map DataLayer.CommandGenerator.generateCommand commands
    
    interface IRobotContext with
        member this.WrappedCommands = wrappedCommands |> Seq.ofList
        
        member this.Robot = robot
        

type RobotRunner(model: IModel, horizontalLines, verticalLines) =
     class
        let mutable currentModel = model
        
        let mutable robot = Robot(IntPoint(0, 0), Direction.Up)
        
        let maze = Maze(horizontalLines, verticalLines)

        let isInitial (element: IElement) = element.Class.Name = "InitialNode"
        
        let isFinal (element: IElement) = element.Class.Name = "FinalNode"
        
        let getInitialNode() =
            try model.Elements |> Seq.filter isInitial |> Seq.exactlyOne
            with :? ArgumentException -> ParserException.raiseWithMessage "Can't find initial node"
            
        let getFinalNode() =
            try model.Elements |> Seq.filter isFinal |> Seq.exactlyOne
            with :? ArgumentException -> ParserException.raiseWithMessage "Can't find final node"

        let findAllEdgesFrom (element: IElement) =
            model.Edges |> Seq.filter (fun (e: IEdge) -> e.From = element)
            
        let next (element: IElement) = let edge = findAllEdgesFrom element |> Seq.exactlyOne in edge.To
        
        let tryGetInitialParsing() =
            try
                let initialNode = getInitialNode()
                let emptyVariableSet = Interpreters.VariableSet.VariableSetFactory.CreateVariableSet([])
                let context = Context.createContext maze robot
                { Variables = emptyVariableSet; Context = context; Model = model; Element = initialNode } |> Some
            with :? ArgumentException -> None
        
        let mutable currentParsing = tryGetInitialParsing()
        
        let mutable isStopped = false
        
        let step (p: Parsing<Context> option) =
                match p with
                | None -> failwith "can not be parsed"
                | Some { Element = element } when element = getFinalNode() ->
                    InterpreterException("Try to parse final node") |> raise
                | _ ->
                    let result = p |> RobotParser.parseRobot
                    if Option.isNone result then failwith "Not parsed: possible unknown operator" else result
            
        member this.Model: IModel = currentModel
        
        member this.CurrentElement =
            match currentParsing with
            | Some p -> p.Element
            | None -> InterpreterException("Can't determine current element") |> raise
        
        member this.Step() =
            match currentParsing with
            | Some _ -> currentParsing <- step currentParsing
            | None -> currentParsing <- tryGetInitialParsing()
                      match currentParsing with
                      | Some _ -> currentParsing <- step currentParsing
                      | None -> InterpreterException("No initial node or there are more than one") |> raise
                      
        member this.Stop() =
            isStopped <- true
            currentParsing <- tryGetInitialParsing()

        member this.Run() =
            let rec run() =
                match currentParsing with
                | Some { Element = element } as result when element = getFinalNode()
                    -> result
                | None -> failwith "Сan not be parsed: not initial node or unknown element"
                | _ ->
                    if (isStopped) then
                        isStopped <- false
                        currentParsing
                    else
                        this.Step()
                        run()
            currentParsing <- tryGetInitialParsing()
            let result = run()
            currentParsing <- result
            
            member this.SpecificContext =
                match currentParsing with
                | None -> invalidOp "No results: this program was not started"
                | Some p ->
                    let commandList = p.Context.Commands
                    RobotContext(robot, commandList) :> IRobotContext
            
            member this.IsEnded =
                match currentParsing with
                | None -> false
                | Some p -> isFinal p.Element 

        interface IProgramRunner<IRobotContext> with
            member this.Model: IModel = this.Model

            member this.SetModel(model: IModel): unit = currentModel <- model

            member this.Run() = this.Run()
            
            member this.Step() = this.Step()
            
            member this.Stop() = this.Stop()
            
            member this.SpecificContext = this.SpecificContext
            
            member this.CurrentElement = this.CurrentElement
            
            member this.IsEnded = this.IsEnded
    end