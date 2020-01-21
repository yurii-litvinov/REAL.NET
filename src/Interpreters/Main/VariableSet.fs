namespace Interpretes

    open Interpreters.Common

    module VariableList =

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

                    member this.RemoveFirst(v: Variable): IVariableSet = 
                        let rec remove v variables =
                            match variables with
                            | [] -> invalidOp "there is no such variable"
                            | h :: t when h = v -> t
                            | _ :: t -> remove v t
                        VariableList(remove v variables) :> IVariableSet

                    member this.RemoveAll(v: Variable): IVariableSet = 
                        VariableList(List.filter ((=) v) variables) :> IVariableSet

                    member this.ToList: Variable list = 
                        raise (System.NotImplementedException())  
            end
    
        
 

