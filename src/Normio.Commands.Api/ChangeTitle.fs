module Normio.Commands.Api.ChangeTitle

open FSharp.Data
open System

open Normio.Core.Commands
open Normio.Commands.Api.CommandHandlers

[<Literal>]
let ChangeTitleJson = """
{
    "changeTitle": {
        "examId": "2a964d85-f503-40a1-8014-2c8ee5ac4a49",
        "newTitle": "newTitle"
    }
}
"""

type ChangeTitleRequest = JsonProvider<ChangeTitleJson>

let (|ChangeTitleRequest|_|) payload =
    try
        let req = ChangeTitleRequest.Parse(payload).ChangeTitle
        (req.ExamId, req.NewTitle) |> Some
    with
        | _ -> None

let validateChangeTitle getExam (req: Guid * string) = async {
    let examId, newTitle = req
    let! exam = getExam examId
    match exam with
    | Some _ -> return Choice1Of2 (examId, newTitle)
    | None -> return Choice2Of2 "Invalid exam id"
}

let changeTitleCommander getExam = {
    Validate = validateChangeTitle getExam
    ToCommand = ChangeTitle
}