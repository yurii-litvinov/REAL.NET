module Languages.Logo.LogoInterpeter

open Interpreters.Common

open Repo

open Languages.Logo

open LogoParser

open LogoSpecific

open Interpreters

open TurtleCommand

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
        
        let initialNode = model.Elements |> Seq.filter (fun e -> e.Class.Name = "InitialNode") |> Seq.exactlyOne
        
        let finalNode = model.Elements |> Seq.filter (fun e -> e.Class.Name = "FinalNode") |> Seq.exactlyOne

        let findAllEdgesFrom (element: IElement) =
            model.Edges |> Seq.filter (fun (e: IEdge) -> e.From = element)
            
        let next (element: IElement) = let edge = findAllEdgesFrom element |> Seq.exactlyOne in edge.To

        interface IProgramRunner<ILogoContext> with
            member this.Model: IModel = currentModel

            member this.SetModel(model: IModel): unit = currentModel <- model

            member this.Run()  =
                let rec run (p: Parsing<Context> option) =
                    match p with
                    | Some (set, context, element) as result when element = finalNode -> result
                    | None -> failwith "can not be parsed"
                    | _ -> p |> LogoParser.parseLogo |> run
                let emtyVariableSet = Interpreters.VariableSet.VariableSetFactory.CreateVariableSet([])
                let context = {Commands = []; Model = model}
                let (wrapped: Parsing<Context> option) = (emtyVariableSet, context, next initialNode) |> Some
                let result = run wrapped
                let context = result.Value |> Parsing.context
                commandList <- context.Commands

            member this.SpicificContext: ILogoContext = 
                let convertedList = List.map convertToLogoCommand commandList
                new LogoContext(convertedList) :> ILogoContext
    end

