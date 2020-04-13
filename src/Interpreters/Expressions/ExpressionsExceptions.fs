namespace Interpreters.Expressions

/// Thrown if lexer failed to recognize input.
type LexerException(input: string) =
    inherit System.Exception(input)

/// Thrown if syntax error occurs.
type SyntaxParserException(input: string) =
    inherit System.Exception(input)

/// Thrown if unexpected error in syntax parser occurs.    
type InnerSyntaxParserException(input: string) =
    inherit System.Exception(input)



