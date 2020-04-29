namespace Interpreters.Logo.LogoInterpeter

open Repo

open Interpreters.Logo

open Interpreters.Logo.LogoParser

open Interpreters.Logo.LogoSpecific

open Interpreters

open TurtleCommand

open System

type LogoContext(list) = 
    class
        interface ILogoContext with
            member this.LogoCommands = List.toSeq list
    end

type LogoRunner(model: IModel) =
    class
        let mutable currentModel = model

        let mutable commandList = []

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
                let context = Context.createContext
                { Variables = emptyVariableSet; Context = context; Model = model; Element = initialNode } |> Some
            with :? ArgumentException -> None
        
        let mutable currentParsing = tryGetInitialParsing()
        
        let mutable isStopped = false
        
        let step (p: Parsing<Context> option) =
                match p with
                | None -> failwith "can not be parsed"
                | Some { Element = element } when element = getFinalNode() ->
                    InterpreterException("Try to parse final node") |> raise
                | _ -> p |> LogoParser.parseLogo
            
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
            let context = result.Value |> Parsing.context
            commandList <- context.Commands

            member this.SpecificContext: ILogoContext = 
                let convertedList = List.map convertToLogoCommand commandList
                new LogoContext(convertedList) :> ILogoContext

        interface IProgramRunner<ILogoContext> with
            member this.Model: IModel = this.Model

            member this.SetModel(model: IModel): unit = currentModel <- model

            member this.Run() = this.Run()
            
            member this.Step() = this.Step()
            
            member this.Stop() = this.Stop()
            
            member this.SpecificContext: ILogoContext = this.SpecificContext
            
            member this.CurrentElement = this.CurrentElement
            
            
    end


    
