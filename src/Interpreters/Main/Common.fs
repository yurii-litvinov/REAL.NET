namespace Interpreters
    
    module Common =
    
        type StandardType = 
            | Int of int
            | Double of double
            | Boolean of bool


        type VariableType = 
            | StandardType

        type MetaData = 
            | NoMeta    

        type Variable = { Name: string; Variable: VariableType; Meta: MetaData }

        type IVariableSet =
            interface 
                abstract member Add: v: Variable -> IVariableSet

                abstract member RemoveFirst: v: Variable -> IVariableSet

                abstract member RemoveAll: v : Variable -> IVariableSet

                abstract member FindByName: name: string -> Variable

                abstract member FindAllByName: name: string -> Variable list

                abstract member ToList: Variable list
            end








