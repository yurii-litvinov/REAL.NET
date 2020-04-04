namespace Interpreters

open System
open Repo

/// Represents primitive types, such as int, double and boolean
type RegularType = 
    | Int of int
    | Double of double
    | Boolean of bool
    | String of string

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
    static member ( == ) (x, y) = x.Value = y.Value

    /// Checks variable for mutability.
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
        | (String _, String _) -> true
        | _ -> false // notice: not implemented for other types
    
    /// Gets the type of regular value.    
    let getType xValue =
        match xValue with
        | Int _ -> PrimitiveTypes.Int
        | Boolean _ -> PrimitiveTypes.Bool
        | Double _ -> PrimitiveTypes.Double
        | String _ -> PrimitiveTypes.String
    
    /// Creates wrapped int value.
    let createInt x = Int x

    /// Creates wrapped double value.
    let createDouble x = Double x
    
    /// Creates wrapped boolean value.
    let createBoolean x = Boolean x
    
    /// Creates wrapped string value.
    let createString x = String x

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
    
    /// Creates wrapped string value.
    let createString x = x |> RegularType.createString |> Regular
    
module MetaData =
    /// Creates meta data.
    let createMeta isMutable (place: (IModel option * IElement option)) =
        let toPlace = PlaceOfCreation
        match isMutable with
        | true -> {Mutability = VariableMutability.Mutable; PlaceOfCreation = (toPlace place)}
        | _ -> {Mutability = VariableMutability.Immutable; PlaceOfCreation = (toPlace place)}

    /// Makes data immutable.
    let makeImmutable (data: MetaData) = {data with Mutability = VariableMutability.Immutable}

    /// Makes data mutable.
    let makeMutable (data: MetaData) = {data with Mutability = VariableMutability.Mutable}

module Variable =
    /// Checks is given variable regular.
    let isRegular variable = VariableValue.isRegular variable.Value

    /// Checks given variables' types for equality.
    let isTypesEqual xVariable yVariable = VariableValue.isTypesEqual xVariable.Value yVariable.Value

    /// Checks given variable for mutability.
    let isMutable (x: Variable) = x.IsMutable

    /// Makes variable mutable.
    let makeMutable variable = {variable with Meta = MetaData.makeMutable variable.Meta}
    
    /// Makes variable immutable.
    let makeImmutable variable = {variable with Meta = MetaData.makeImmutable variable.Meta} 

    /// Creates int variable with given name, value and meta.
    let createInt name x place = {Name = name; Value = (VariableValue.createInt x); Meta = MetaData.createMeta true place}
    
    /// Creates double variable with given name, value and meta.
    let createDouble name x place = {Name = name; Value = (VariableValue.createDouble x); Meta = MetaData.createMeta true place}

    /// Creates boolean variable with given name, value and meta.
    let createBoolean name x place = {Name = name; Value = (VariableValue.createBoolean x); Meta = MetaData.createMeta true place}
    
    /// Creates string variable with given name, value and meta.
    let createString name x place = {Name = name; Value = (VariableValue.createString x); Meta = MetaData.createMeta true place}

    /// Changes value of given variable.
    let changeValue (newValue: VariableValue) (variable: Variable) =
        if (not variable.IsMutable) then invalidOp "Variable is immutable"
        elif (VariableValue.isTypesEqual newValue variable.Value) then {variable with Value = newValue}
        else invalidOp "Not equal types"
    /// Changes value of given value in spite of constraints.
    let forceChangeValue (newValue: VariableValue) (variable: Variable) = {variable with Value = newValue}
    
    

