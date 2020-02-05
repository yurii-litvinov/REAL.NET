namespace Interpreters

type ParserException(message: string, place: PlaceOfCreation) =
    class
        inherit System.Exception(message)

        new(message) = new ParserException(message, EmptyPlace)

        member this.PlaceWhereRaised = place
    end

module ParserException =
    
    let raise message = new ParserException(message)

    let raiseWithPlace message place = new ParserException(message, place)

