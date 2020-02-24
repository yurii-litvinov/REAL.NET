namespace Interpreters.Expressions

open Interpreters

type IStringExpressionParser =
    interface
        abstract member Parse : set: IVariableSet -> stringExpression: string -> VariableValue
    
        abstract member TryParse: set: IVariableSet -> stringExpression: string -> VariableValue option
    end    
    