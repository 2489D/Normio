module Normio.Commands.Api.AddStudent

open System
open FSharp.Data
open Normio.Core
open Normio.Core.Commands
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

let validateAddStudent getExamByExamId (examId, studentId, name) = async {
    let! exam = getExamByExamId examId
    match exam with
    | Some _ ->
        match name |> UserName40.create with
        | Ok name40 ->
            return Choice1Of2 (examId, ({ Id = studentId; Name = name40 }: Student))
        | Error err -> return Choice2Of2 (err.ToString())
    | _ -> return Choice2Of2 "Invalid Exam Id"
}

let addStudentCommander getExamByExamId = {
    Validate = validateAddStudent getExamByExamId
    ToCommand = AddStudent
}
