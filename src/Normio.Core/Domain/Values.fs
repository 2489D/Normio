namespace Normio.Core.Domain

type ExamTitle40 = private ExamTitle40 of string

type ExamTitle40 with
    member this.Value = this |> fun (ExamTitle40 title) -> title

module ExamTitle40 =
    let create title =
        if title |> String.length > 40
        then StringTooLong "Exam Title should be less than 40" |> Error
        else ExamTitle40 title |> Ok
        

type UserName40 = private UserName40 of string

type UserName40 with
    member this.Value = this |> fun (UserName40 name) -> name

module UserName40 =
    let create name =
        if name |> String.length > 40
        then StringTooLong "User Name should be less than 40" |> Error
        else UserName40 name |> Ok
        

type FileString200 = private FileString200 of string

type FileString200 with
    member this.Value = this |> fun (FileString200 s) -> s

module FileString200 =
    let create s =
        if s |> String.length > 200
        then StringTooLong "File String should be less than 200" |> Error
        else FileString200 s |> Ok

