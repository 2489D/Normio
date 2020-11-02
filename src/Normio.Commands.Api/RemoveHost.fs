[<AutoOpen>]
module Normio.Commands.Api.RemoveHost

open System
open System.Text.Json.Serialization
open Normio.Core.Commands
open Normio.Commands.Api.CommandHandlers

[<CLIMutable>]
type RemoveHostRequest =
    {
        [<JsonPropertyName("examId")>]
        ExamId : Guid
        [<JsonPropertyName("studentId")>]
        HostId : Guid
    }

let validateRemoveHost req = async {
    return Ok (req.ExamId, req.HostId)
}

let removeHostCommander = {
    Validate = validateRemoveHost
    ToCommand = RemoveHost
}
