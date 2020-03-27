namespace Interpreters.Expressions

open System

type BinaryOperator =
    | PlusOp
    | MinusOp
    | MultiplyOp
    | DivideOp
    | EqualityOp
    | InequalityOp
    
type UnaryOperator =
    | Negative
    | Not
    
type PrimitiveTypes =
    | Bool = 0
    | Int = 1
    | Double = 2
    | String = 3

type Terminal =
    | IntConst of int
    | BoolConst of bool
    | DoubleConst of double
    | StringConst of string
    | Name of string
    | BinOperator of BinaryOperator 
    | UnOperator of UnaryOperator
    | Comma
    | OpeningRoundBracket
    | ClosingRoundBracket
    | OpeningSquareBracket
    | ClosingSquareBracket
    | OpeningCurlyBracket
    | ClosingCurlyBracket
    | NewOperator
    | TypeSelection of PrimitiveTypes
    