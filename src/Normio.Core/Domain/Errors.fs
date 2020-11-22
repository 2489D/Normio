namespace Normio.Core.Domain

[<AutoOpen>]
module Errors =
    type DomainError =
        | EmptyString of context:string
        | StringTooLong of context:string
        | WrongFormat of context:string

        override this.ToString () =
            match this with
            | EmptyString ctx -> sprintf "String is empty: %A" ctx
            | StringTooLong ctx -> sprintf "String is too long : %A" ctx
            | WrongFormat ctx -> sprintf "String is not in a right format: %A" ctx
