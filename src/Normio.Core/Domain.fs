module Normio.Core.Domain

open System

type EntityBuilder<'a, 'b> = {
    Create: 'a -> Result<'b, string>
    ToRaw: 'b -> 'a
}

type ExamTitle40 = ExamTitle40 of string

let examTitle40 = {
    Create = fun title ->
        if title |> String.length > 40
        then "Title too long" |> Error
        else ExamTitle40 title |> Ok
    ToRaw = fun (ExamTitle40 title) -> title
}

type UserName40 = UserName40 of string

let userName40 = {
    Create = fun name ->
        if name |> String.length > 40
        then "Name too long" |> Error
        else UserName40 name |> Ok
    ToRaw = fun (UserName40 name) -> name
}

type FileString40 = FileString40 of string

let fileString40 = {
    Create = fun s ->
        if s |> String.length > 40
        then "Too long" |> Error
        else FileString40 s |> Ok
    ToRaw = fun (FileString40 s) -> s
}

type Student = {
    Id: Guid
    Name: string
}

type Host = {
    Id: Guid
    Name: string
}

type File = {
    Id: Guid
    Name: string
}

type Submission = {
    Id: Guid
    Exam: Guid
    Student: Student
    File: File
}

type Exam = {
    Id: Guid
    Title: string
    Questions: File list
    Students: Map<Guid, Student>
    Hosts: Map<Guid, Host>
}
