module Normio.Commands.Api.StartExam

open FSharp.Data
open Normio.Commands
open Normio.ReadModels
open Normio.CommandHandlers

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

let validateStartExam getExam examId = async {
    let! exam = getExam examId
    match exam with
    | Some exam ->
        match exam.Status with
        | BeforeExam -> return Choice1Of2 examId
        | _ -> return Choice2Of2 "Exam already started"
    | None -> return Choice2Of2 "Invalid Exam Id"
}

let startExamCommander getExam = {
    Validate = validateStartExam getExam
    ToCommand = StartExam
}
