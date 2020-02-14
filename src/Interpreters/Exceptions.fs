namespace Interpreters

type ParserException(message: string, place: PlaceOfCreation) =
    class
        inherit System.Exception(message)

        new(message) = new ParserException(message, PlaceOfCreation.empty)

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
            | _ -> invalidOp "No model"
    end

module ParserException =
    
    let raise message = new ParserException(message)

    let raiseWithPlace message place = new ParserException(message, place)

