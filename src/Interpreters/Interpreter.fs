namespace Interpreters

open Repo

type IPeformerCommand =
    interface
        abstract member Description: string with get
    end

type IInterpeter<'T> = 
    interface
        abstract member SpicificContext: 'T with get

        abstract member CurrentElement: IElement with get

        abstract member Step : unit

        abstract member StepInto : unit

        abstract member Debug : unit
        
        abstract member BreakPoints : IElement list with get, set

        abstract member Run: unit
    end

