module Normio.Commands.Api.RemoveHost

open FSharp.Data
open Normio.Core.Commands
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

let validateRemoveHost getExamByExamId (examId, hostId) = async {
    let! exam = getExamByExamId examId
    match exam with
    | Some _ -> return Choice1Of2 (examId, hostId)
    | _ -> return Choice2Of2 "Invalid Exam Id"
}

let removeHostCommander getExamByExamId = {
    Validate = validateRemoveHost getExamByExamId
    ToCommand = RemoveHost
}
