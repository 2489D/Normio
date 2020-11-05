namespace Normio.Timer

[<AutoOpen>]
module Errors =
    type TimerError =
        | CannotSetTimer of context:string

        override this.ToString () =
            match this with
                | CannotSetTimer ctx -> sprintf "Cannot set timer: %A" ctx
