module Normio.Commands.Api.ChangeTitle

open FSharp.Data
open Normio.Core.Domain
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

let validateChangeTitle getExamByExamId (examId, newTitle) = async {
    let! exam = getExamByExamId examId
    match exam with
    | Some _ -> 
        match examTitle40.Create newTitle with
        | Ok title -> return Choice1Of2 (examId, title)
        | Error err -> return Choice2Of2 (err |> DomainError.toString)
    | _ -> return Choice2Of2 "Invalid Exam Id"
}

let changeTitleCommander getExamByExamId = {
    Validate = validateChangeTitle getExamByExamId
    ToCommand = ChangeTitle
}
