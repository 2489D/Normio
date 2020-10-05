module Normio.Commands.Api.CloseExam

open FSharp.Data
open Normio.Core.Commands
open Normio.Core.States
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

let validateCloseExam getState examId = async {
    let! state = getState examId
    match state with
    | ExamIsFinished _ -> return Choice1Of2 examId
    | _ -> return Choice2Of2 "Exam is not finished"
}

let closeExamCommander getState = {
    Validate = validateCloseExam getState
    ToCommand = CloseExam
}
