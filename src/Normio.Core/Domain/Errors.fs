namespace Normio.Core.Domain

type DomainError =
    | StringTooLong of context:string

    override this.ToString () =
        match this with
        | StringTooLong ctx -> sprintf "String is too long : %A" ctx
