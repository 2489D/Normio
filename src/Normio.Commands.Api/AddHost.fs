module Normio.Commands.Api.AddHost

open System
open FSharp.Data
open Normio.Core
open Normio.Core.Commands
open Normio.Commands.Api.CommandHandlers

[<Literal>]
let AddHostJson = """ {
"addHost" : {
        "examId" : "2a964d85-f503-40a1-8014-2c8ee5ac4a49",
        "name" : "John"
    }
}
"""

type AddHostRequest = JsonProvider<AddHostJson>

let (|AddHostRequest|_|) payload =
    try
        let req = AddHostRequest.Parse(payload).AddHost
        (req.ExamId, Guid.NewGuid(), req.Name) |> Some
    with
    | _ -> None

let validateAddHost getExamByExamId (examId, hostId, name) = async {
    let! exam = getExamByExamId examId
    match exam with
    | Some _ ->
        match name |> UserName40.create with
        | Ok name40 ->
            return Choice1Of2 (examId, ({ Id = hostId; Name = name40 }: Host))
        | Error err -> return Choice2Of2 (err.ToString())
    | _ -> return Choice2Of2 "Invalid Exam Id"
}

let addHostCommander getExamByExamId = {
    Validate = validateAddHost getExamByExamId
    ToCommand = AddHost
}
