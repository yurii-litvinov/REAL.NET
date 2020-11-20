namespace Interpreters

open Interpreters

type IVariableSet =
    interface 
        abstract member Add: v: Variable -> IVariableSet

        abstract member Remove: v: Variable -> IVariableSet

        abstract member RemoveAll: v: Variable -> IVariableSet

        abstract member FindByName: name: string -> Variable

        abstract member FindAllByName: name: string -> Variable list
        
        abstract member Filter: (Variable -> bool) -> Variable list

        abstract member ChangeValue: v: Variable -> newValue: VariableValue -> IVariableSet

        abstract member ChangeValueByName: name: string -> newValue: VariableValue -> IVariableSet

        abstract member Drop: unit -> IVariableSet

        abstract member ToList: Variable list
    end








