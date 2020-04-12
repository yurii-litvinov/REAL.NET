module Interpreters.Expressions.Lexemes

open Interpreters

type BinOp =
    /// Operator +.
    | PlusOp
    /// Operator -.
    | MinusOp
    /// Operator *.
    | MultiplyOp
    /// Operator /.
    | DivideOp
    /// Operator ==.
    | EqualityOp
    /// Operator !=.
    | InequalityOp
    /// Operator >.
    | BiggerOp
    /// Operator <.
    | LessOp
    /// Operator >=.
    | BiggerOrEqualOp
    /// Operator <=.
    | LessOrEqualOp
    /// Logical and.
    | AndOp
    /// Logical or.
    | OrOp
    /// Assigment operator =.
    | AssigmentOp

type UnOp =
     // Unary operators.
    /// Negative operator -, for example, -a.
    | NegativeOp
    /// Operator !, logical not.
    | Not
    
type ExtraInfo =
    | NumberOfArgs of int
    
/// Type which represents token.
type Terminal =
    // Constants
    /// Const of int type.
    | IntConst of int
    /// Const of bool type.
    | BoolConst of bool
    /// Const of double type.
    | DoubleConst of double
    /// Const of string type.
    | StringConst of string
    // ---------------
    // Types and variables.
    /// Variable.
    | VariableName of string
    /// Function.
    | FunctionName of string
    /// Array.
    | ArrayName of string
    /// Type declaration, such as int, double, string or bool
    | TypeSelection of PrimitiveTypes
    /// New operator.
    | NewOperator
    | BinOp of BinOp
    | UnOp of UnOp
    // Brackets.
    /// (.
    | OpeningRoundBracket
    /// ).
    | ClosingRoundBracket
    /// [.
    | OpeningSquareBracket
    /// ].
    | ClosingSquareBracket
    /// {.
    | OpeningCurlyBracket
    /// }.
    | ClosingCurlyBracket
    //-----------------
    // Special symbols    
    /// Comma ,.
    | Comma
    | ExtraInfo of ExtraInfo
  
    