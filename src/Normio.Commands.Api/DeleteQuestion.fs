module Normio.Commands.Api.DeleteQuestion

open System
open FSharp.Data
open Normio.Commands.Api
open Normio.Core.Domain
open Normio.Core.Commands
open Normio.Commands.Api.CommandHandlers
open Normio.Storage.ReadModels

[<Literal>]
let DeleteQuestionJson = """ {
"deleteQuestion" : {
        "examId" : "2a964d85-f503-40a1-8014-2c8ee5ac4a49",
        "fileId" : "2a964d85-f503-40a1-8014-2c8ee5ac4a49"
    }
}
"""

type DeleteQuestionRequest = JsonProvider<DeleteQuestionJson>

let (|DeleteQuestionRequest|_|) payload =
    try
        let req = DeleteQuestionRequest.Parse(payload).DeleteQuestion
        (req.ExamId, req.FileId) |> Some
    with
    | _ -> None

let validateDeleteQuestion getExamByExamId (examId, fileId) = async {
    let! exam = getExamByExamId examId
    match exam with
    | Some exam' ->
        return Choice1Of2 (examId, fileId)
    | _ -> return Choice2Of2 "Invalid Exam Id"
}

let createDeleteQuestionCommander getExamByExamId = {
    Validate = validateDeleteQuestion getExamByExamId
    ToCommand = DeleteQuestion
}