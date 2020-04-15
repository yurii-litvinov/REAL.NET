namespace Interpreters

type PrimitiveTypes =
    | Int = 0
    | Double = 1
    | Bool = 2
    | String = 3
    
type ArrayTypes =
    | IntArray = 0
    | DoubleArray = 1
    | BoolArray = 2
    | StringArray = 3
    
type ExpressionType =
    | PrimitiveCase of PrimitiveTypes
    | ArrayCase of ArrayTypes
    | VoidCase
    
    
