namespace Interpreters

open System
open Repo

/// Represents primitive types, such as int, double and boolean
type RegularType = 
    | Int of int
    | Double of double
    | Boolean of bool

/// Represents a value, could be regular or complex
type VariableValue = 
    | Regular of RegularType
    | Complex

/// Mutability of variable.
type VariableMutability =
    | Mutable = 0
    | Immutable = 1

/// Block where variable was created or its absence. 
type PlaceOfCreation = 
    | PlaceOfCreation of IModel option * IElement option

/// Meta info about variable, such as mutability, place of creation.    
type MetaData = {Mutability: VariableMutability; PlaceOfCreation: PlaceOfCreation} with
    member this.IsMutable = this.Mutability = VariableMutability.Mutable 

/// Represents a variable, contains name, value and meta.
type Variable = { Name: string; Value: VariableValue; Meta: MetaData } with
    /// Checks values of given variables for equality.
    static member (==) (x, y) = x.Value = y.Value

    /// Checks variable's mutability.
    member this.IsMutable = this.Meta.IsMutable

module PlaceOfCreation =
    /// Creates none block PlaceOfCreation.
    let empty = PlaceOfCreation(None, None)

module RegularType =
    /// Checks that types of given values are equal.
    let isTypesEqual xValue yValue = 
        match (xValue, yValue) with
        | (Int _, Int _) -> true
        | (Boolean _, Boolean _) -> true
        | (Double _, Double _) -> true
        | _ -> false // notice: not implemented for other types
    
    /// Creates wrapped int value.
    let createInt x = Int x

    /// Creates wrapped double value.
    let createDouble x = Double x
    
    /// Creates wrapped boolean value.
    let createBoolean x = Boolean x

module VariableValue =
    /// Checks is given type regular.
    let isRegular (value: VariableValue) =
        match value with
        | Regular _ -> true
        | _ -> false
    
    /// Checks that types of given values are equal.
    let isTypesEqual xValue yValue =   
        match (xValue, yValue) with
        | (Regular xv, Regular yv) -> RegularType.isTypesEqual xv yv
        | _ -> new NotImplementedException() |> raise
    
    /// Creates wrapped int value.
    let createInt x = x |> RegularType.createInt |> Regular

    /// Creates wrapped double value.
    let createDouble x = x |> RegularType.createDouble |> Regular

    /// Creates wrapped boolean value.
    let createBoolean x = x |> RegularType.createBoolean |> Regular

module MetaData =

    let createMeta isMutable (place: (IModel option * IElement option)) =
        let toPlace = PlaceOfCreation
        match isMutable with
        | true -> {Mutability = VariableMutability.Mutable; PlaceOfCreation = (toPlace place)}
        | _ -> {Mutability = VariableMutability.Immutable; PlaceOfCreation = (toPlace place)}

    let makeImmutable (data: MetaData) = {data with Mutability = VariableMutability.Immutable}

    let makeMutable (data: MetaData) = {data with Mutability = VariableMutability.Mutable}

module Variable =

    let isRegular variable = VariableValue.isRegular variable.Value

    let isTypesEqual xVariable yVariable = VariableValue.isTypesEqual xVariable.Value yVariable.Value

    let isMutable (x: Variable) = x.IsMutable

    let makeMutable variable = {variable with Meta = MetaData.makeMutable variable.Meta}
    
    let makeImmutable variable = {variable with Meta = MetaData.makeImmutable variable.Meta} 

    let createInt name x place = {Name = name; Value = (VariableValue.createInt x); Meta = MetaData.createMeta true place}
    
    let createDouble name x place = {Name = name; Value = (VariableValue.createDouble x); Meta = MetaData.createMeta true place}

    let createBoolean name x place = {Name = name; Value = (VariableValue.createBoolean x); Meta = MetaData.createMeta true place}

    let changeValue (newValue: VariableValue) (variable: Variable) =
        if (not variable.IsMutable) then invalidOp "Variable is immutable"
        elif (VariableValue.isTypesEqual newValue variable.Value) then {variable with Value = newValue}
        else invalidOp "Not equal types"
    
    let forceChangeValue (newValue: VariableValue) (variable: Variable) = {variable with Value = newValue}
    
    

