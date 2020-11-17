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
            // TODO : json
            [<JsonPropertyName("startDateTime")>]
            StartDateTime : DateTime
            [<JsonPropertyName("durationMins")>]
            DurationMins : float
        }

    let validateOpenExam req = async {
        let titleRes = req.Title |> ExamTitle40.create
        let isStartTimePast = req.StartDateTime <= DateTime.UtcNow
        let duration = TimeSpan.FromMinutes req.DurationMins
        let isDurationNegative = duration <= TimeSpan.Zero
        match (titleRes, isStartTimePast, isDurationNegative) with
        | (Error err, _, _) -> return Error (err.ToString())
        | (_, true, _) -> return Error "The start time is past"
        | (_, _, true) -> return Error "The duration is not positive"
        | (Ok title, _, _) -> return Ok (title, req.StartDateTime, duration)
    }

    let toOpenExamCommand (title, startTime, duration) =
        OpenExam (Guid.NewGuid(), title, startTime, duration)

    let openExamCommander = {
        Validate = validateOpenExam
        ToCommand = toOpenExamCommand
    }
