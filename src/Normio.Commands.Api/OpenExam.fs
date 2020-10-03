module Normio.Commands.Api.OpenExam

open System
open FSharp.Data

open Normio.CommandHandlers
open Normio.Commands
open Normio.Domain


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
        {
            Id = Guid.NewGuid()
            Title = req.Title
            Students = Map.empty
            Hosts = Map.empty
            Questions = []
        }
        |> Some
    with
        | _ -> None

let validateOpenExam exam = async {
    if exam.Title.Length > 40
    then return Choice2Of2 "The title of an exam should be less than 40"
    else return Choice1Of2 exam
}

let openExamToCommand exam =
    OpenExam (exam.Id, exam.Title)

let openExamCommander = {
    Validate = validateOpenExam
    ToCommand = openExamToCommand
}
