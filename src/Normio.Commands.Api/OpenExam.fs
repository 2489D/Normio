module Normio.Commands.Api.OpenExam

open System
open FSharp.Data

open Normio.Domain
open Normio.Commands
open Normio.CommandHandlers

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
    match title.Length with
    | l when l > 40 ->
        return Choice2Of2 "The title of an exam should be less than 40"
    | _ -> return Choice1Of2 (id, title)
}

let openExamCommander = {
    Validate = validateOpenExam
    ToCommand = OpenExam
}
