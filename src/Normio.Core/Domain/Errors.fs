namespace Normio.Core

[<AutoOpen>]
module Errors =
    type DomainError =
        | EmptyString of context:string
        | StringTooLong of context:string

        override this.ToString () =
            match this with
            | EmptyString ctx -> sprintf "String is empty: %A" ctx
            | StringTooLong ctx -> sprintf "String is too long : %A" ctx
