module Normio.Commands.Api.AddHost

open System
open FSharp.Data
open Normio.Core.Domain
open Normio.Core.States
open Normio.Core.Commands
open Normio.Storage.ReadModels
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
        (req.ExamId, ({ Id = Guid.NewGuid(); Name = req.Name}: Host)) |> Some
    with
    | _ -> None

let validateAddHost getStatus (req: Guid * Host) = async {
    let examId, host = req
    let! state = getStatus examId
    match state with
    | ExamIsWaiting exam ->
            return Choice1Of2 (examId, host)
    | _ -> return Choice2Of2 "Exam is not waiting"
}

let addHostCommander getState = {
    Validate = validateAddHost getState
    ToCommand = AddHost
}
