module Normio.Commands.Api.CloseExam

open FSharp.Data
open Normio.Core.Commands
open Normio.Storage.ReadModels
open Normio.Commands.Api.CommandHandlers

[<Literal>]
let CloseExamJson = """ {
"closeExam" : {
    "examId" : "2a964d85-f503-40a1-8014-2c8ee5ac4a49"
    }
}
"""

type CloseExamRequest = JsonProvider<CloseExamJson>

let (|CloseExamRequest|_|) payload =
    try
        let req = CloseExamRequest.Parse(payload).CloseExam
        req.ExamId |> Some
    with
    | _ -> None

let validateCloseExam getExam examId = async {
    let! exam = getExam examId
    match exam with
    | Some exam ->
        match exam.Status with
        | AfterExam -> return Choice1Of2 examId
        | _ -> return Choice2Of2 "Cannot close exam unless the exam is over"
    | None -> return Choice2Of2 "Invalid Exam Id"
}

let closeExamCommander getExam = {
    Validate = validateCloseExam getExam
    ToCommand = CloseExam
}
