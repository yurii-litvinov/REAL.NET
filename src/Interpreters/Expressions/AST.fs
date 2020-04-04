module Interpreters.Expressions.AST

open Interpreters

type AbstractSyntaxNode =
    | ConstOfInt of int
    | ConstOfDouble of double
    | ConstOfBool of bool
    | ConstOfString of string
    | Function of string * AbstractSyntaxNode list
    | Plus of AbstractSyntaxNode * AbstractSyntaxNode
    | Minus of AbstractSyntaxNode * AbstractSyntaxNode
    | Multiply of AbstractSyntaxNode * AbstractSyntaxNode
    | Divide of AbstractSyntaxNode * AbstractSyntaxNode
    | Assigment of Variable * AbstractSyntaxNode
    | Equality of AbstractSyntaxNode * AbstractSyntaxNode
    | Inequality of AbstractSyntaxNode * AbstractSyntaxNode
    | LogicalOr of AbstractSyntaxNode * AbstractSyntaxNode
    | LogicalAnd of AbstractSyntaxNode * AbstractSyntaxNode
    | Variable of string 
    | IndexAt of string * AbstractSyntaxNode
    | ArrayDeclaration of PrimitiveTypes * int * AbstractSyntaxNode list
      
  

