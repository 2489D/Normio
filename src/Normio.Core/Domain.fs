module Normio.Core.Domain

open System

type DomainError =
    | StringTooLong of context:string

module DomainError =
    let toString = function
        | StringTooLong ctx -> sprintf "String Too Long: %A" ctx

type EntityBuilder<'TValue, 'TValidated> = {
    Create: 'TValue -> Result<'TValidated, DomainError>
    ToRaw: 'TValidated -> 'TValue
}

type ExamTitle40 = ExamTitle40 of string

let examTitle40 = {
    Create = fun title ->
        if title |> String.length > 40
        then StringTooLong "Exam Title should be less than 40" |> Error
        else ExamTitle40 title |> Ok
    ToRaw = fun (ExamTitle40 title) -> title
}

type UserName40 = UserName40 of string

let userName40 = {
    Create = fun name ->
        if name |> String.length > 40
        then StringTooLong "User Name should be less than 40" |> Error
        else UserName40 name |> Ok
    ToRaw = fun (UserName40 name) -> name
}

type FileString200 = FileString200 of string

let fileString200 = {
    Create = fun s ->
        if s |> String.length > 200
        then StringTooLong "File String should be less than 200" |> Error
        else FileString200 s |> Ok
    ToRaw = fun (FileString200 s) -> s
}

type Student = {
    Id: Guid
    Name: UserName40
}

type Host = {
    Id: Guid
    Name: UserName40
}

type File = {
    Id: Guid
    Name: FileString200
}

type Submission = {
    Id: Guid
    ExamId: Guid
    Student: Student
    File: File
}

type Exam = {
    Id: Guid
    Title: ExamTitle40
    Questions: File list
    Submissions: Submission list
    Students: Map<Guid, Student>
    Hosts: Map<Guid, Host>
}
