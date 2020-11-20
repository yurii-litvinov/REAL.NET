﻿namespace Interpreters

    module VariableSet =
        
        open Interpreters.Common
        
        type VariableSetFactory =
            struct
                static member CreateVariableSet (variables: seq<Variable>) = new VariableList(variables) :> IVariableSet
            
                static member CreateVariableSet variables = new VariableList(variables) :> IVariableSet
            end 
            


    
        
 

