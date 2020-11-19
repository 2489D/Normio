namespace Normio.Commands.Api

open System
open System.Text.Json.Serialization

open Normio.Core
open Normio.Core.Commands

[<AutoOpen>]
module LetHostIn =
    [<CLIMutable>]
    type LetHostInRequest =
        { [<JsonPropertyName("examId")>]
          ExamId: Guid
          [<JsonPropertyName("hostId")>]
          HostId: Guid
          [<JsonPropertyName("name")>]
          Name: string }

    let validateLetHostInRequest req =
        async {
            match req.Name |> UserName40.Create with
            | Ok name -> return Ok(req.ExamId, req.HostId, name)
            | Error err -> return Error <| err.ToString()
        }

    let toLetHostInCommand (examId, hostId, hostName) =
        let host: Host = { Id = hostId; Name = hostName }
        LetHostIn(examId, host)

    let letHostInCommander =
        { Validate = validateLetHostInRequest
          ToCommand = toLetHostInCommand }
