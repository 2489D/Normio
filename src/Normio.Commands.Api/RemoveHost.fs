module Normio.Commands.Api.RemoveHost

open System
open FSharp.Data
open Normio.Core.States
open Normio.Core.Commands
open Normio.Storage.ReadModels
open Normio.Commands.Api.CommandHandlers

[<Literal>]
let RemoveHostJson = """ {
"removeHost" : {
        "examId" : "2a964d85-f503-40a1-8014-2c8ee5ac4a49",
        "hostId" : "2a964d85-f503-40a1-8014-2c8ee5ac4a49"
    }
}
"""

type RemoveHostRequest = JsonProvider<RemoveHostJson>

let (|RemoveHostRequest|_|) payload =
    try
        let req = RemoveHostRequest.Parse(payload).RemoveHost
        (req.ExamId, req.HostId) |> Some
    with
    | _ -> None

let validateRemoveHost getState (req: Guid * Guid) = async {
    let examId, hostId = req
    let! state = getState examId
    match state with
    | ExamIsWaiting exam ->
        if exam.Hosts |> Map.containsKey hostId
        then return Choice1Of2 (examId, hostId)
        else return Choice2Of2 "Invalid Host Id"
    | _ -> return Choice2Of2 "Exam is not waiting"
}

let removeHostCommander getState = {
    Validate = validateRemoveHost getState
    ToCommand = RemoveHost
}
