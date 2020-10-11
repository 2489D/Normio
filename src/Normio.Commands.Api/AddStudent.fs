module Normio.Commands.Api.AddStudent

open System
open FSharp.Data
open Normio.Core.Domain
open Normio.Core.States
open Normio.Core.Commands
open Normio.Storage.ReadModels
open Normio.Commands.Api.CommandHandlers

[<Literal>]
let AddStudentJson = """ {
"addStudent" : {
        "examId" : "2a964d85-f503-40a1-8014-2c8ee5ac4a49",
        "name" : "John"
    }
}
"""

type AddStudentRequest = JsonProvider<AddStudentJson>

let (|AddStudentRequest|_|) payload =
    try
        let req = AddStudentRequest.Parse(payload).AddStudent
        (req.ExamId, Guid.NewGuid(), req.Name) |> Some
    with
    | _ -> None

let validateAddStudent getExamByExamId (examId, stuId, name) = async {
    let! exam = getExamByExamId examId
    match exam with
    | Some _ ->
        match name |> userName40.Create with
        | Ok name40 ->
            return Choice1Of2 (examId, ({ Id = stuId; Name = name40 }: Student))
        | Error msg -> return Choice2Of2 (msg |> DomainError.toString)
    | _ -> return Choice2Of2 "Invalid Exam Id"
}

let addStudentCommander getState = {
    Validate = validateAddStudent getState
    ToCommand = AddStudent
}
