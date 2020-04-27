namespace Interpreters

open System

/// Represents a value, could be regular or complex
type ExpressionValue = 
    | RegularValue of RegularType
    | ArrayValue of ArrType
    | Void

module ExpressionValue =
    /// Checks is given type regular.
    let isRegular (value: ExpressionValue) =
        match value with
        | RegularValue _ -> true
        | _ -> false
    
    let isArray (value: ExpressionValue) =
        match value with
        | ArrayValue _ -> true
        | _ -> false
        
    let isVoid (value: ExpressionValue) = value = Void
    
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
    
    let initArr n value =
        match value with
        | RegularValue v -> ArrType.initArray n v |> ArrayValue
        | _ -> invalidOp "Value is not regular"
    
    let tryCreateArr list =
        let rec extract list output =
            match list with
            | (RegularValue h) :: t -> extract t (h :: output)
            | [] -> List.rev output |> Some
            | _ -> None
        match extract list [] with
        | Some l ->
            match ArrType.tryCreateArray l with
            | Some arr -> arr |> ArrayValue |> Some
            | None -> None
        | None -> None
        
    let tryChangeValueAtIndex index value arr =
        match arr with
        | ArrayValue a ->
            match value with
            | RegularValue v ->
                try (ArrType.changeValueAtIndex index v a |> ArrayValue |> Some)
                with
                | :? IndexOutOfRangeException -> None
                | :? InvalidOperationException -> None
            | _ -> None
        | _ -> None
                
    let tryGetValueAtIndex index arr =
        match arr with
        | ArrayValue a ->
            try ArrType.getValueAtIndex index a |> RegularValue |> Some
            with
            | :? IndexOutOfRangeException -> None
        | _ -> None
        
            
                
        

