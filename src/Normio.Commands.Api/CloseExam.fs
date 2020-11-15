namespace Normio.Commands.Api

open System
open System.Text.Json.Serialization
open Normio.Core.Commands

[<AutoOpen>]
module CloseExam =
    [<CLIMutable>]
    type CloseExamRequest =
        {
            [<JsonPropertyName("examId")>]
            ExamId : Guid
        }

    let validateCloseExam req = async {
        return Ok req.ExamId
    }

    let closeExamCommander = {
        Validate = validateCloseExam
        ToCommand = CloseExam
    }
