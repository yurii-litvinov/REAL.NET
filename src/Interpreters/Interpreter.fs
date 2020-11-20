namespace Interpreters

open Repo

type IPerformerCommand =
    interface
        abstract member Description: string with get
    end

type IProgramRunner<'T> =
    interface
        abstract member SpecificContext: 'T with get

        abstract member Run: unit -> unit

        abstract member Model: IModel with get

        abstract member SetModel: model: IModel -> unit
    end

type IInterpreter<'T> = 
    interface
        inherit IProgramRunner<'T>

        abstract member CurrentElement: IElement with get

        abstract member Step : unit -> unit

        abstract member StepInto : unit -> unit

        abstract member Debug : unit -> unit
        
        abstract member BreakPoints : IElement list with get, set        

        abstract member Continue: unit -> unit

        abstract member Stop: unit -> unit
    end

