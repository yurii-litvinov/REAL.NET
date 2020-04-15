namespace Interpreters.Expressions

open System
open Interpreters

type State = State of string

type FunctionCollection = Map<string, (ExpressionType list * ExpressionType * (VariableValue list -> State -> (VariableValue * State))) list>

type EnvironmentOfExpressions(vars: IVariableSet, functions: FunctionCollection, state: State, place: PlaceOfCreation) =
    struct
        new(vars, functions, state) = EnvironmentOfExpressions(vars, functions, state, PlaceOfCreation (None, None)) 
        
        member this.Variables = vars
        
        member this.Functions = functions
        
        member this.State = state
        
        member this.Place = place
        
        member this.AddVariable var =
            let newVars = vars.Add var
            EnvironmentOfExpressions(newVars, this.Functions, this.State, this.Place)
            
        static member op_Equality(x: EnvironmentOfExpressions, y: EnvironmentOfExpressions) = (x.Variables = y.Variables) && (x.State = y.State)
            
        member this.ChangeValue var newVal =
            let newVars = vars.ChangeValue var newVal
            EnvironmentOfExpressions(newVars, this.Functions, this.State, this.Place)
            
        override this.ToString() = "Vars: " + (this.Variables.ToList.ToString()) + " State: " + this.State.ToString()
    end
    
module StandardFunctions =
    module Available =
        let private wrap f l s = (f l, s)
        
        let getIntAbs =
            let functionArgs = [ PrimitiveCase PrimitiveTypes.Int ]
            let functionEval v =
                match v with
                | [ RegularValue (Int value) ] ->
                    (if value > 0 then value else -value) |> Int |> RegularValue 
                | _ -> "Not int" |> TypeException |> raise
            (functionArgs, PrimitiveCase PrimitiveTypes.Int, wrap functionEval)
            
        let getDoubleAbs =
            let functionArgs = [ PrimitiveCase PrimitiveTypes.Double ]
            let functionEval v =
                match v with
                | [ RegularValue (Double value) ] ->
                    (if value > 0.0 then value else -value) |> Double |> RegularValue
                | _ -> "Not double" |> TypeException |> raise
            (functionArgs, PrimitiveCase PrimitiveTypes.Double, wrap functionEval)
            
        let getRound =
            let functionArgs = [ PrimitiveCase PrimitiveTypes.Double ]
            let functionEval v =
                match v with
                | [ RegularValue (Double value) ] ->
                    let rounded = Math.Round(value) |> (int)
                    rounded |> Int |> RegularValue
                | _ -> "Not double" |> TypeException |> raise
            (functionArgs, PrimitiveCase PrimitiveTypes.Int, wrap functionEval)
    
    open Available        
    let getAllFunctions =
        let round = ("round", [ getRound ])
        let abs = ("abs", [ getIntAbs; getDoubleAbs ])
        let funcs = [ round; abs ]
        Map(funcs)
           
module EnvironmentOfExpressions =
    let getStandardEnvironment =
        let vars = VariableSet.VariableSetFactory.CreateVariableSet([])
        let functions = StandardFunctions.getAllFunctions
        let state = State("")
        EnvironmentOfExpressions(vars, functions, state)
           
