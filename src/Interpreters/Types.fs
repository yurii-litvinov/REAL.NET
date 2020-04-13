namespace Interpreters

type PrimitiveTypes =
    | Void = 0
    | Int = 1
    | Double = 2
    | Bool = 3
    | String = 4
    
type ArrayTypes =
    | IntArray = 0
    | DoubleArray = 1
    | BoolArray = 2
    | StringArray = 3
    
type Types =
    | PrimitiveTypes
    | ArrayTypes
    
