namespace Interpreters.Expressions

/// Thrown if lexer failed to recognize input.
type LexerException(input: string) =
    inherit System.Exception(input)

/// Thrown if syntax error occurs.
type SyntaxParserException(input: string) =
    inherit System.Exception(input)

/// Thrown if type error occurs.
type TypeException(message: string) =
    inherit System.Exception(message)    
    
/// Thrown when evaluator fails.
type EvaluatorException(message: string) =
    inherit System.Exception(message)

