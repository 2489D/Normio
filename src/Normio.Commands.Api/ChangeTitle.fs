namespace Normio.Commands.Api

open System
open System.Text.Json.Serialization
open Normio.Core
open Normio.Core.Commands
open Normio.Commands.Api.CommandHandlers

[<AutoOpen>]
module ChangeTitle =
    [<CLIMutable>]
    type ChangeTitleRequest =
        {
            [<JsonPropertyName("examId")>]
            ExamId : Guid
            [<JsonPropertyName("title")>]
            Title : string
        }

    let validateChangeTitle req = async {
        match req.Title |> ExamTitle40.Create with
        | Ok title -> return Ok (req.ExamId, title)
        | Error err -> return err.ToString() |> Error
    }

    let changeTitleCommander = {
        Validate = validateChangeTitle
        ToCommand = ChangeTitle
    }
