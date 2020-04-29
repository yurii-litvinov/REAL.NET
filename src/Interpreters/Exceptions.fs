namespace Interpreters

/// Thrown if type error occurs.
type TypeException(message: string) =
    inherit System.Exception(message)   

type ParserException(message: string, place: PlaceOfCreation, innerException) =
    class
        inherit System.Exception(message, innerException)

        new(message, place) = new ParserException(message, place, null)
        
        new(message) = new ParserException(message, PlaceOfCreation.empty, null)

        member this.PlaceWhereRaised = place

        member this.IsRaisedInModel = 
            match place with
            | PlaceOfCreation(Some _, _) -> true
            | _ -> false

        member this.ModelWhereRaised = 
            match place with
            | PlaceOfCreation(Some m, _) -> m
            | _ -> invalidOp "No model"

        member this.IsRaisedInElement = 
            match place with
            | PlaceOfCreation(_, Some _) -> true
            | _ -> false

        member this.ElementWhereRaised = 
            match place with
            | PlaceOfCreation(_, Some e) -> e
            | _ -> invalidOp "No element"
    end

module ParserException =
    
    let raiseAll message place innerException = new ParserException(message, place, innerException) |> raise
    
    let raiseWithMessage message = new ParserException(message) |> raise
    
    let raiseWithInner message (innerException: exn) = new ParserException(message, PlaceOfCreation.empty, innerException) |> raise

    let raiseWithPlace (message: string) (place: PlaceOfCreation) =  new ParserException(message) |> raise
    
type InterpreterException(message, innerException: exn) =
    inherit System.Exception(message, innerException)
    
    new(message) = InterpreterException(message, null)

