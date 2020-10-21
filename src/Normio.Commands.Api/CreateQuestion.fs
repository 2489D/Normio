module Normio.Commands.Api.CreateQuestion

open System
open FSharp.Data
open Normio.Commands.Api
open Normio.Core.Domain
open Normio.Core.Commands
open Normio.Commands.Api.CommandHandlers
open Normio.Persistence.ReadModels

[<Literal>]
let CreateQuestionJson = """ {
    "createQuestion" : {
        "examId" : "2a964d85-f503-40a1-8014-2c8ee5ac4a49",
        "fileString" : "path-to-string"
    }
}
"""

type CreateQuestionRequest = JsonProvider<CreateQuestionJson>

let (|CreateQuestionRequest|_|) payload =
    try
        let req = CreateQuestionRequest.Parse(payload).CreateQuestion
        (req.ExamId, (Guid.NewGuid(), req.FileString )) |> Some
    with
    | _ -> None

let validateCreateQuestion getExamByExamId (examId, fileId, fileName) = async {
    let! exam = getExamByExamId examId
    match exam with
    | Some exam' ->
        match fileName |> FileString200.create with
        | Ok fileName' ->
            let question = {
                Id = fileId
                FileName = fileName'
            }
            return Choice1Of2 (examId, question)
        | Error err ->
            return Choice2Of2 (err.ToString())
    | _ -> return Choice2Of2 "Invalid Exam Id"
}

let createCreateQuestionCommander getExamByExamId = {
    Validate = validateCreateQuestion getExamByExamId
    ToCommand = CreateQuestion
}


