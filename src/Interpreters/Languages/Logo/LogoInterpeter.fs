module Languages.Logo.LogoInterpeter

open Interpreters.Common

open Repo

open Languages.Logo

open LogoSpecific

open Interpreters

type LogoInterpreter(model: IModel) =
    class
         interface IInterpeter<LogoContext> with
             member this.BreakPoints
                 with get (): IElement list = 
                     raise (System.NotImplementedException())
                 and set (v: IElement list): unit = 
                     raise (System.NotImplementedException())

             member this.CurrentElement: IElement = 
                 raise (System.NotImplementedException())

             member this.Debug: unit = 
                 raise (System.NotImplementedException())
             member this.Run: unit = 
                 raise (System.NotImplementedException())

             member this.SpicificContext: LogoContext = 
                 raise (System.NotImplementedException())

             member this.Step: unit = 
                 raise (System.NotImplementedException())

             member this.StepInto: unit = 
                 raise (System.NotImplementedException())  
            
    end

