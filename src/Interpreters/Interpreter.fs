namespace Interpreters

open Repo

type IPerformerCommand =
    interface
        abstract member Description: string with get
    end

type IProgramRunner<'T> =
    interface
        abstract member SpecificContext: 'T with get

        abstract member CurrentElement: IElement with get
        
        abstract member Run: unit -> unit
       
        abstract member Step: unit -> unit
        
        abstract member IsEnded: bool
        
        abstract member Stop: unit -> unit

        abstract member Model: IModel with get

        abstract member SetModel: model: IModel -> unit
    end

