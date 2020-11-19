namespace Normio.Commands.Api

open System
open System.Text.Json.Serialization

open Normio.Core.Commands

[<AutoOpen>]
module LetHostOut =
    [<CLIMutable>]
    type LetHostOutRequest =
        {
            [<JsonPropertyName("examId")>]
            ExamId : Guid
            [<JsonPropertyName("hostId")>]
            HostId : Guid
        }

    let validateLetHostOutRequest req =
        async {
            return Ok (req.ExamId, req.HostId)
        }

    let toLetHostOutCommand (examId, hostId) =
        LetHostOut(examId, hostId)

    let letHostOutCommander =
        { Validate = validateLetHostOutRequest
          ToCommand = toLetHostOutCommand }

