namespace Interpreters.Expressions

open Interpreters

type IStringExpressionParser =
    interface
        abstract member Parse : env: EnvironmentOfExpressions -> stringExpression: string -> EnvironmentOfExpressions * ExpressionValue
    
        abstract member TryParse: env: EnvironmentOfExpressions -> stringExpression: string -> Result<(EnvironmentOfExpressions * ExpressionValue), exn> 
    end        

type StringExpressionParser() =
    static member Create() = StringExpressionParser() :> IStringExpressionParser 
    
    interface IStringExpressionParser with
        member this.TryParse env stringExpression =
            try
                let lexemes = Lexer.parseString stringExpression
                let tree = SyntaxParser.parseLexemes lexemes
                let (newEnv, returnValue) = Evaluator.evaluate env tree
                Ok (newEnv, returnValue)
            with
            | :? LexerException as e -> Error (e :> exn)
            | :? SyntaxParserException as e -> Error (e :> exn)
            | :? TypeException as e -> Error (e :> exn)
            | :? EvaluatorException as e -> Error (e :> exn)
            
        member this.Parse env stringExpression =
            let parsedOption = (this :> IStringExpressionParser).TryParse env stringExpression
            match parsedOption with
            | Ok res -> res
            | Error e -> raise e
            
            