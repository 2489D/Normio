module Normio.Commands.Api.CloseExam

open FSharp.Data
open Normio.Core.Commands
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

let validateCloseExam getExamByExamId examId = async {
    let! state = getExamByExamId examId
    match state with
    | Some _ -> return Choice1Of2 examId
    | _ -> return Choice2Of2 "Invalid Exam Id"
}

let closeExamCommander getExamByExamId = {
    Validate = validateCloseExam getExamByExamId
    ToCommand = CloseExam
}
