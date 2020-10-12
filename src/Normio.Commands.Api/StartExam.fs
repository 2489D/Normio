module Normio.Commands.Api.StartExam

open FSharp.Data
open Normio.Core.Commands
open Normio.Commands.Api.CommandHandlers

[<Literal>]
let StartExamJson = """ {
"startExam" : {
    "examId" : "2a964d85-f503-40a1-8014-2c8ee5ac4a49"
    }
}
"""

type StartExamRequest = JsonProvider<StartExamJson>

let (|StartExamRequest|_|) payload =
    try
        let req = StartExamRequest.Parse(payload).StartExam
        req.ExamId |> Some
    with
    | _ -> None

let validateStartExam getExamByExamId examId = async {
    let! exam = getExamByExamId examId
    match exam with
    | Some _ -> return Choice1Of2 examId
    | _ -> return Choice2Of2 "Invalid Exam Id"
}

let startExamCommander getExamByExamId = {
    Validate = validateStartExam getExamByExamId
    ToCommand = StartExam
}
