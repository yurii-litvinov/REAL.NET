module Interpreters.TypeConversion

open System.Reflection

let tryInt value =
    match value with
    | RegularValue (Int x) -> Some x
    | _ -> None
    
let tryDouble value =
    match value with
    | RegularValue (Double x) -> Some x
    | _ -> None
    
let tryBool value =
    match value with
    | RegularValue (Bool x) -> Some x
    | _ -> None
    
let tryString value =
    match value with
    | RegularValue (String x) -> Some x
    | _ -> None