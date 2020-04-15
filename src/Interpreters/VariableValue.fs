namespace Interpreters

/// Represents a value, could be regular or complex
type VariableValue = 
    | RegularValue of RegularType
    | ArrayValue of ArrType
    | Void

module VariableValue =
    /// Checks is given type regular.
    let isRegular (value: VariableValue) =
        match value with
        | RegularValue _ -> true
        | _ -> false
    
    let isArray (value: VariableValue) =
        match value with
        | ArrayValue _ -> true
        | _ -> false
        
    let isVoid (value: VariableValue) = value = Void
    
    let getType value =
        match value with
        | RegularValue v -> RegularType.getType v |> PrimitiveCase
        | ArrayValue v -> ArrType.getType v |> ArrayCase
        | Void -> VoidCase
        
    /// Checks that types of given values are equal.
    let isTypesEqual xValue yValue =   
        match (xValue, yValue) with
        | (RegularValue xv, RegularValue yv) -> RegularType.isTypesEqual xv yv
        | (ArrayValue xv, ArrayValue yv) -> ArrType.isTypesEqual xv yv
        | (Void, Void) -> true
        | _ -> false
        
    /// Creates wrapped int value.
    let createInt x = x |> RegularType.createInt |> RegularValue

    /// Creates wrapped double value.
    let createDouble x = x |> RegularType.createDouble |> RegularValue

    /// Creates wrapped boolean value.
    let createBoolean x = x |> RegularType.createBoolean |> RegularValue
    
    /// Creates wrapped string value.
    let createString x = x |> RegularType.createString |> RegularValue


