module Normio.Commands.Api.EndExam

open FSharp.Data
open Normio.Core.Commands
open Normio.Core.States
open Normio.Storage.ReadModels
open Normio.Commands.Api.CommandHandlers

[<Literal>]
let EndExamJson = """ {
"endExam" : {
    "examId" : "2a964d85-f503-40a1-8014-2c8ee5ac4a49"
    }
}
"""

type EndExamRequest = JsonProvider<EndExamJson>

let (|EndExamRequest|_|) payload =
    try
        let req = EndExamRequest.Parse(payload).EndExam
        req.ExamId |> Some
    with
    | _ -> None

let validateEndExam getState examId = async {
    let! state = getState examId
    match state with
    | ExamIsRunning _ -> return Choice1Of2 examId
    | _ -> return Choice2Of2 "Exam is not running"
}

let endExamCommander getState = {
    Validate = validateEndExam getState
    ToCommand = EndExam
}
