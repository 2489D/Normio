module Normio.Commands.Api.OpenExam

open System
open FSharp.Data

open Normio.Core.Domain
open Normio.Core.Commands
open Normio.Commands.Api.CommandHandlers

[<Literal>]
let OpenExamJson = """ {
"openExam" : {
    "title": "exam title"
    }
}
"""

type OpenExamRequest = JsonProvider<OpenExamJson>

let (|OpenExamRequest|_|) payload =
    try
        let req = OpenExamRequest.Parse(payload).OpenExam
        (Guid.NewGuid(), req.Title) |> Some
    with
    | _ -> None

let validateOpenExam (id, (title: string)) = async {
    match title |> examTitle40.Create with
    | Ok title40 -> return Choice1Of2 (id, title40)
    | Error msg -> return Choice2Of2 msg
}

let openExamCommander = {
    Validate = validateOpenExam
    ToCommand = OpenExam
}
