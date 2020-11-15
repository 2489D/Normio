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
            if len > 40
            then StringTooLong "Exam Title should be less than or equal to 40" |> Error
            else if len = 0 || isNull title
            then EmptyString "Exam Title should not be empty" |> Error
            else ExamTitle40 title |> Ok

    [<JsonFSharpConverter>]
    type UserName40 =
        private | UserName40 of string
        member this.Value = this |> fun (UserName40 name) -> name

    module UserName40 =
        let create name =
            let len = name |> String.length
            if len > 40
            then StringTooLong "User Name should be less than or equal to 40" |> Error
            else if len <= 0 || isNull name
            then EmptyString "User Name should not be empty" |> Error
            else UserName40 name |> Ok

    type MessageContent =
        private | MessageContent of string
        
        static member Create content =
            let len = content |> String.length
            if len = 0 || isNull content
            then EmptyString "Message Content should not be empty" |> Error
            else MessageContent content |> Ok
