module Normio.Commands.Api.RemoveHost

open System
open FSharp.Data
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

let validateRemoveHost getExam (req: Guid * Guid) = async {
    let examId, hostId = req
    let! exam = getExam examId
    match exam with
    | Some exam ->
        match exam.Status with
        | BeforeExam ->
            if exam.Hosts |> Map.containsKey hostId
            then return Choice1Of2 (examId, hostId)
            else return Choice2Of2 "Invalid Host Id"
        | _ -> return Choice2Of2 "A host can leave only before the exam starts"
    | None -> return Choice2Of2 "Invalid Exam Id"
}

let removeHostCommander getExam = {
    Validate = validateRemoveHost getExam
    ToCommand = RemoveHost
}
