module Interpreters.Expressions.TypeTransform

open System

let tryDouble (x: string) =
    try double x |> Some
    with :? FormatException -> None
    
let tryInt (x: string) =
    try int x |> Some
    with :? FormatException -> None

let tryBool (x: string) =
    match x with
    | "true" -> Some true
    | "false" -> Some false
    | _ -> None

