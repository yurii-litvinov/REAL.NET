module Interpreters.Expressions.AST

open Interpreters

type ExtraNode =
    | ArgsNumber of int
    | TypeNode of PrimitiveTypes

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
    | Assigment of AbstractSyntaxNode * AbstractSyntaxNode
    | Equality of AbstractSyntaxNode * AbstractSyntaxNode
    | Inequality of AbstractSyntaxNode * AbstractSyntaxNode
    | Negative of AbstractSyntaxNode
    | LogicalOr of AbstractSyntaxNode * AbstractSyntaxNode
    | LogicalAnd of AbstractSyntaxNode * AbstractSyntaxNode
    | LogicalNot of AbstractSyntaxNode
    | Variable of string 
    | IndexAt of string * AbstractSyntaxNode
    | ArrayDeclaration of PrimitiveTypes * AbstractSyntaxNode * AbstractSyntaxNode list
    | Temp of ExtraNode
  

