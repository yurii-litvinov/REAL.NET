namespace Interpreters

open Repo

type IPeformerCommand =
    interface
        abstract member Description: string with get
    end

type IInterpeter = 
    interface
        abstract member Run: model: Repo.IModel -> List<IPeformerCommand>
    end

