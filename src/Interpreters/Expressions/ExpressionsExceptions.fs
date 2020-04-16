namespace Interpreters.Expressions

/// Thrown if lexer failed to recognize input.
type LexerException(input: string) =
    inherit System.Exception(input)

/// Thrown if syntax error occurs.
type SyntaxParserException(input: string) =
    inherit System.Exception(input) 
    
/// Thrown when evaluator fails.
type EvaluatorException(message: string, innerException: exn) =
    inherit System.Exception(message, innerException)
    
    new(message) = EvaluatorException(message, null)

