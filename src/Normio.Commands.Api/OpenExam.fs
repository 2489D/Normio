namespace Normio.Commands.Api

open System
open System.Text.Json.Serialization
open Normio.Core
open Normio.Core.Commands

[<AutoOpen>]
module OpenExam =
    [<CLIMutable>]
    type OpenExamRequest =
        {
            [<JsonPropertyName("title")>]
            Title : string
            [<JsonPropertyName("startDateTime")>]
            StartDateTime : DateTime option
        }

    let validateOpenExam req = async {
        match req.Title |> ExamTitle40.create with
        | Ok title40 -> return Ok title40
        | Error err -> return Error (err.ToString())
    }

    let toOpenExamCommand title =
        OpenExam (Guid.NewGuid(), title)

    let openExamCommander = {
        Validate = validateOpenExam
        ToCommand = toOpenExamCommand
    }
