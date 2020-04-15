namespace Interpreters

/// Represents primitive types, such as int, double and boolean
type RegularType = 
    | Int of int
    | Double of double
    | Bool of bool
    | String of string
    
module RegularType =
    /// Gets the type of regular value.    
    let getType xValue =
        match xValue with
        | Int _ -> PrimitiveTypes.Int
        | Bool _ -> PrimitiveTypes.Bool
        | Double _ -> PrimitiveTypes.Double
        | String _ -> PrimitiveTypes.String
        
    
    /// Checks that types of given values are equal.
    let isTypesEqual xValue yValue = getType xValue = getType yValue
    
    /// Creates wrapped int value.
    let createInt x = Int x

    /// Creates wrapped double value.
    let createDouble x = Double x
    
    /// Creates wrapped boolean value.
    let createBoolean x = Bool x
    
    /// Creates wrapped string value.
    let createString x = String x

