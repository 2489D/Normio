module Normio.Commands.Api.StartExam

open FSharp.Data
open Normio.Core.States
open Normio.Core.Commands
open Normio.Storage.ReadModels
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

let validateStartExam getState examId = async {
    let! state = getState examId
    match state with
    | ExamIsWaiting _ -> return Choice1Of2 examId
    | _ -> return Choice2Of2 "Exam is not waiting"
}

let startExamCommander getState = {
    Validate = validateStartExam getState
    ToCommand = StartExam
}
