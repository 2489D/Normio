[<AutoOpen>]
module Normio.Commands.Api.AddHost

open System
open System.Text.Json.Serialization
open Normio.Core
open Normio.Core.Commands
open Normio.Commands.Api.CommandHandlers

[<CLIMutable>]
type AddHostRequest =
    {
        [<JsonPropertyName("examId")>]
        ExamId : Guid
        [<JsonPropertyName("name")>]
        Name : string
    }

let validateAddHost req = async {
    match req.Name |> UserName40.create with
    | Ok name40 ->
        return Ok (req.ExamId, name40)
    | Error err -> return err.ToString() |> Error
}

let toAddHostCommand (examId, name) =
    let host =
        { Id = Guid.NewGuid()
          Name = name }
    AddHost (examId, host)

let addHostCommander = {
    Validate = validateAddHost
    ToCommand = toAddHostCommand
}
