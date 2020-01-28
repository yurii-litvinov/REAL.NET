namespace Interpreters

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

type PlaceOfCreation = PlaceOfCreation of IModel * IElement
    
type MetaData = {Mutability: VariableMutability; PlaceOfCreation: PlaceOfCreation} 

type Variable = { Name: string; Value: VariableValue; Meta: MetaData } with
    static member (==) (x, y) = x.Value = y.Value

module RegularType =
    let isTypesEqual xValue yValue = 
        match (xValue, yValue) with
        | (Int _, Int _) -> true
        | (Boolean _, Boolean _) -> true
        | (Double _, Double _) -> true
        | _ -> false // notice: not implemented for other types

module VariableValue =
    let isRegular (value: VariableValue) =
        match value with
        | Regular _ -> true
        | _ -> false

    let isTypesEqual xValue yValue =   
        match (xValue, yValue) with
        | (Regular xv, Regular yv) -> RegularType.isTypesEqual xv yv
        | _ -> false

module Variable =

    let isRegular variable = VariableValue.isRegular variable.Value

    let isTypesEqual xVariable yVariable = VariableValue.isTypesEqual xVariable.Value yVariable.Value

    let changeValue (newValue: VariableValue) (variable: Variable) = 
        if (VariableValue.isTypesEqual newValue variable.Value) then {variable with Value = newValue}
        else invalidOp "not equal types"

    
    

