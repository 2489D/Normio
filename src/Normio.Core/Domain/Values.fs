namespace Normio.Core

open System.Text.Json.Serialization

[<AutoOpen>]
module Values =
    [<JsonFSharpConverter>]
    type ExamTitle40 =
        private | ExamTitle40 of string
        member this.Value = this |> fun (ExamTitle40 title) -> title

    module ExamTitle40 =
        let create title =
            let len = title |> String.length
            if len > 40 || len <= 0
            then StringTooLong "Exam Title should be less than 40 and non empty" |> Error
            else ExamTitle40 title |> Ok

    [<JsonFSharpConverter>]
    type UserName40 =
        private | UserName40 of string
        member this.Value = this |> fun (UserName40 name) -> name

    module UserName40 =
        let create name =
            let len = name |> String.length
            if len > 40 || len <= 0
            then StringTooLong "User Name should be less than 40 and non empty" |> Error
            else UserName40 name |> Ok
