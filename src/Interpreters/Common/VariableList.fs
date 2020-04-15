module Interpreters.Common

open Interpreters

type VariableList(variables: Variable list) =
    struct
        new(variables: seq<Variable>) = VariableList(Seq.toList variables)

        interface IVariableSet with
          
            member this.Add(v: Variable): IVariableSet = 
                VariableList(v :: variables) :> IVariableSet

            member this.FindAllByName(name: string): Variable list = 
                List.filter (fun x -> x.Name = name) variables

            member this.FindByName(name: string): Variable = 
                List.find (fun x-> x.Name = name) variables
                
            member this.TryFindByName(name: string): Variable option =
                List.tryFind (fun x-> x.Name = name) variables

            member this.Remove(v: Variable): IVariableSet = 
                let rec remove v variables =
                    match variables with
                    | [] -> invalidOp "there is no such variable"
                    | h :: t when h = v -> t
                    | _ :: t -> remove v t
                new VariableList(remove v variables) :> IVariableSet

            member this.RemoveAll(v: Variable): IVariableSet = 
                new VariableList(List.filter ((=) v) variables) :> IVariableSet

            member this.ChangeValue(v: Variable) (newValue: VariableValue): IVariableSet = 
                let rec change list result =
                    match list with
                    | h :: t -> 
                        if v = h then (List.rev result) @ ((Variable.changeValue newValue v) :: t)
                        else change t (h :: result)
                    | [] -> invalidOp "there is no such variable"
                new VariableList(change variables []) :> IVariableSet
            
            member this.ChangeValueByName(name: string) (newValue: VariableValue): IVariableSet = 
                                  raise (System.NotImplementedException())
            
            member this.Drop(): IVariableSet = new VariableList(variables.Tail) :> IVariableSet
            
            member this.Filter filter = List.filter filter variables

            member this.ToList: Variable list = variables                  
    end