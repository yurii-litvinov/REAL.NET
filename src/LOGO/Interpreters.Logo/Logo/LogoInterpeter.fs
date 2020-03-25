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
        
        let getInitialNode() = model.Elements |> Seq.filter (fun e -> e.Class.Name = "InitialNode") |> Seq.exactlyOne
        
        let getFinalNode() = model.Elements |> Seq.filter (fun e -> e.Class.Name = "FinalNode") |> Seq.exactlyOne

        let findAllEdgesFrom (element: IElement) =
            model.Edges |> Seq.filter (fun (e: IEdge) -> e.From = element)
            
        let next (element: IElement) = let edge = findAllEdgesFrom element |> Seq.exactlyOne in edge.To

        member this.Model: IModel = currentModel

        member this.Run()  =
            let rec run (p: Parsing<Context> option) =
                match p with
                | Some { Variables = set; Context = context; Model = model; Element = element} as result when element = getFinalNode() -> result
                | None -> failwith "can not be parsed"
                | _ -> p |> LogoParser.parseLogo |> run
            let emtyVariableSet = Interpreters.VariableSet.VariableSetFactory.CreateVariableSet([])
            let context = Context.createContext
            let firstBlock = 
                try getInitialNode() |> next
                with :? ArgumentException -> ParserException.raiseException "Can't detect initial node or there are more than one edge from it"
            let (wrapped: Parsing<Context> option) = {Variables = emtyVariableSet; Context = context; Model = model; Element = firstBlock} |> Some
            let result = run wrapped
            let context = result.Value |> Parsing.context
            commandList <- context.Commands

            member this.SpecificContext: ILogoContext = 
                let convertedList = List.map convertToLogoCommand commandList
                new LogoContext(convertedList) :> ILogoContext

        interface IProgramRunner<ILogoContext> with
            member this.Model: IModel = this.Model

            member this.SetModel(model: IModel): unit = currentModel <- model

            member this.Run() = this.Run()

            member this.SpecificContext: ILogoContext = this.SpecificContext
    end

module Test = 

    let repo = RepoFactory.Create()
    let model = repo.CreateModel("some", "LogoMetamodel")

    let runner = new LogoRunner(model)
    
