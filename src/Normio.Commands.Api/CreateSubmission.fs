module Normio.Commands.Api.CreateSubmission

open System
open FSharp.Data
open Normio.Commands.Api
open Normio.Core.Domain
open Normio.Core.Commands
open Normio.Commands.Api.CommandHandlers
open Normio.Persistence.ReadModels

[<Literal>]
let CreateSubmissionJson = """ {
"createSubmission" : {
        "examId" : "2a964d85-f503-40a1-8014-2c8ee5ac4a49",
        "studentId" : "2a964d85-f503-40a1-8014-2c8ee5ac4a49",
        "fileString" : "adfkljb121231khj231"
    }
}
"""

type CreateSubmissionRequest = JsonProvider<CreateSubmissionJson>

let (|CreateSubmissionRequest|_|) payload =
    try
        let req = CreateSubmissionRequest.Parse(payload).CreateSubmission
        (Guid.NewGuid(), req.ExamId, req.StudentId, (Guid.NewGuid(), req.FileString )) |> Some
    with
    | _ -> None

let validateCreateSubmission getExamByExamId (submissionId, examId, studentId, file) = async {
    let! exam = getExamByExamId examId
    match exam with
    | Some exam' ->
        if exam'.Students |> Array.exists (fun s -> s.Id = studentId)
        then
            let (fileId, fileString) = file
            match fileString |> FileString200.create with
            | Ok fileString -> 
                let submission = {
                    Id = submissionId
                    ExamId = examId
                    Student = exam'.Students |> Array.find (fun s -> s.Id = studentId)
                    File = {
                        Id = fileId
                        FileName = fileString
                    }
                }
                return Choice1Of2 (examId, submission)
            | Error err ->
                return Choice2Of2 (err.ToString())
        else return Choice2Of2 "Invalid Student Id"
    | _ -> return Choice2Of2 "Invalid Exam Id"
}

let createSubmissionCommander getExamByExamId = {
    Validate = validateCreateSubmission getExamByExamId
    ToCommand = CreateSubmission
}
