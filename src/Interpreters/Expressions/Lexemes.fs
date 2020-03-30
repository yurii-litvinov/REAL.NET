namespace Interpreters.Expressions

open Interpreters
    
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
    | Name of string
    /// Type declaration, such as int, double, string or bool
    | TypeSelection of PrimitiveTypes
    /// New operator.
    | NewOperator
    // ----------------
    // Binary Operators
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
    /// Assigment operator =.
    | AssigmentOp
    // ----------------
    // Unary operators.
    /// Negative operator -, for example, -a.
    | Negative
    /// Operator !, logical not.
    | Not
    // ----------------
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
  
    