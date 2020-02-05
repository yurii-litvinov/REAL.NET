namespace Interpreters

open System
open Repo

type RegularType = 
    | Int of int
    | Double of double
    | Boolean of bool

type VariableValue = 
    | Regular of RegularType
    | Complex

type VariableMutability =
    | Mutable = 0
    | Immutable = 1

type PlaceOfCreation = 
    | PlaceOfCreation of IModel * IElement
    | EmptyPlace
    
type MetaData = {Mutability: VariableMutability; PlaceOfCreation: PlaceOfCreation} with
    member this.IsMutable = this.Mutability = VariableMutability.Mutable 

type Variable = { Name: string; Value: VariableValue; Meta: MetaData } with
    static member (==) (x, y) = x.Value = y.Value

    member this.IsMutable = this.Meta.IsMutable

module RegularType =
    
    let isTypesEqual xValue yValue = 
        match (xValue, yValue) with
        | (Int _, Int _) -> true
        | (Boolean _, Boolean _) -> true
        | (Double _, Double _) -> true
        | _ -> false // notice: not implemented for other types

    let createInt x = Int x

    let createDouble x = Double x
    
    let createBoolean x = Boolean x

module VariableValue =
    
    let isRegular (value: VariableValue) =
        match value with
        | Regular _ -> true
        | _ -> false

    let isTypesEqual xValue yValue =   
        match (xValue, yValue) with
        | (Regular xv, Regular yv) -> RegularType.isTypesEqual xv yv
        | _ -> new NotImplementedException() |> raise

    let createInt x = x |> RegularType.createInt |> Regular

    let createDouble x = x |> RegularType.createDouble |> Regular

    let createBoolean x = x |> RegularType.createBoolean |> Regular

module MetaData =

    let createMeta isMutable (place: (IModel * IElement) option) =
        let toPlace = function
            | None -> EmptyPlace
            | Some(m, e) -> PlaceOfCreation(m, e)
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

    
    

