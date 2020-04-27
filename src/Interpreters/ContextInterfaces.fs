namespace Interpreters

type IPrintable<'T> =
    interface
        abstract member State: IStateConsole
        
        abstract member NewState: IStateConsole -> 'T
    end

