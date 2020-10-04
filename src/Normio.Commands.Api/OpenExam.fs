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
    let len = title |> String.length 
    if len <= 40
    then return Choice1Of2 (id, title)
    else return Choice2Of2 "The title of an exam should be less than 40"
}

let openExamCommander = {
    Validate = validateOpenExam
    ToCommand = OpenExam
}
