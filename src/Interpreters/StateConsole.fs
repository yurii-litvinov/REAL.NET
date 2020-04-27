namespace Interpreters

open System
    
type IStateConsole =
    interface
        abstract member Print: message: string -> IStateConsole
        
        abstract member PrintLn: message: string -> IStateConsole
        
        abstract member Output: string
        
        abstract member Read: unit -> (string * IStateConsole)
        
        abstract member ReadLn: unit -> (string * IStateConsole)
                
        abstract member Input: string
    end    
    
type private StateConsole(input: string, output: string) = 
    interface IStateConsole with
        member this.Print message = StateConsole(input, output + message) :> IStateConsole
        
        member this.PrintLn message = StateConsole(input, output + message + "\n") :> IStateConsole
        
        member this.Output = output
        
        member this.Read() =
            let index = Math.Min(input.IndexOf(' '), input.IndexOf('\n'))
            if index = -1 then (input, this :> IStateConsole)
            else (input.Substring(0, index + 1), StateConsole(input.Substring(index + 1), output) :> IStateConsole)
            
        member this.ReadLn() =
            let index = input.IndexOf('\n')
            if index = -1 then (input, this :> IStateConsole)
            else (input.Substring(0, index + 1), StateConsole(input.Substring(index + 1), output) :> IStateConsole)
            
        member this.Input = input
        
module StateConsole =
    let empty = StateConsole(String.Empty, String.Empty) :> IStateConsole
    
    let printLn message (console: IStateConsole) = console.PrintLn message
    
    let print message (console: IStateConsole) = console.Print message
    
    let readLn (console: IStateConsole) = console.ReadLn 
    
    let read (console: IStateConsole) = console.Read
    
    

        


