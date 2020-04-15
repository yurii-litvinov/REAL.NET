namespace Interpreters

open System
open Repo
open System.Collections.Immutable    


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
    
    override this.ToString() = this.Name + " " + this.Value.ToString()

module PlaceOfCreation =
    /// Creates none block PlaceOfCreation.
    let empty = PlaceOfCreation(None, None)
    
    let extract place =
        match place with
        | PlaceOfCreation (x, y) -> (x, y)

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
    
    let getType variable = VariableValue.getType variable.Value 

    /// Checks given variable for mutability.
    let isMutable (x: Variable) = x.IsMutable

    /// Makes variable mutable.
    let makeMutable variable = {variable with Meta = MetaData.makeMutable variable.Meta}
    
    /// Makes variable immutable.
    let makeImmutable variable = {variable with Meta = MetaData.makeImmutable variable.Meta} 

    /// Creates int variable with given name, value and standard meta.
    let createInt name x place = {Name = name; Value = (VariableValue.createInt x); Meta = MetaData.createMeta true place}
    
    /// Creates double variable with given name, value and standard meta.
    let createDouble name x place = {Name = name; Value = (VariableValue.createDouble x); Meta = MetaData.createMeta true place}

    /// Creates boolean variable with given name, value and standard meta.
    let createBoolean name x place = {Name = name; Value = (VariableValue.createBoolean x); Meta = MetaData.createMeta true place}
    
    /// Creates string variable with given name, value and standard meta.
    let createString name x place = {Name = name; Value = (VariableValue.createString x); Meta = MetaData.createMeta true place}
    
    /// Create variable with given name, value and standard meta
    let createVar name x place = {Name = name; Value = x; Meta = MetaData.createMeta true place}

    /// Changes value of given variable.
    let changeValue (newValue: VariableValue) (variable: Variable) =
        if (not variable.IsMutable) then invalidOp "Variable is immutable"
        elif (VariableValue.isTypesEqual newValue variable.Value) then {variable with Value = newValue}
        else invalidOp "Not equal types"
    /// Changes value of given value in spite of constraints.
    let forceChangeValue (newValue: VariableValue) (variable: Variable) = {variable with Value = newValue}
    
    

