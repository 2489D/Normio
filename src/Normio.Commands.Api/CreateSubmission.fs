module Normio.Commands.Api.CreateSubmission

open System
open FSharp.Data
open Normio.Commands.Api
open Normio.Core.Domain
open Normio.Core.Commands
open Normio.Commands.Api.CommandHandlers
open Normio.Storage.ReadModels

[<Literal>]
let CreateSubmissionJson = """ {
"createSubmission" : {
        "examId" : "2a964d85-f503-40a1-8014-2c8ee5ac4a49",
        "studentId" : "2a964d85-f503-40a1-8014-2c8ee5ac4a49",
        "fileString" : "adfkljb121231khj231"
    }
}
"""

type RemoveHostRequest = JsonProvider<CreateSubmissionJson>

let (|RemoveHostRequest|_|) payload =
    try
        let req = RemoveHostRequest.Parse(payload).CreateSubmission
        (Guid.NewGuid(), req.ExamId, req.StudentId, {| Id = Guid.NewGuid(); Name = req.FileString |}) |> Some
    with
    | _ -> None

let validateRemoveHost getExamByExamId (submissionId, examId, studentId, file) = async {
    let! exam = getExamByExamId examId
    match exam with
    | Some exam' ->
        if exam'.Students |> Map.containsKey studentId
        then
            match file |> fileString200.Create with
            | Ok fileString -> 
                let submission = {
                    Id = submissionId
                    ExamId = examId
                    Student = exam'.Students |> Map.find studentId
                    File = {
                        Id = Guid.NewGuid()
                        Name = fileString
                    }
                }
                return Choice1Of2 (examId, submission)
            | Error err ->
                return Choice2Of2 (err |> DomainError.toString)
        else return Choice2Of2 "Invalid Student Id"
    | _ -> return Choice2Of2 "Invalid Exam Id"
}

let removeHostCommander getExamByExamId = {
    Validate = validateRemoveHost getExamByExamId
    ToCommand = CreateSubmission
}
