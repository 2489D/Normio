namespace Normio.Commands.Api

open System
open System.Text.Json.Serialization
open Normio.Core.Commands

[<AutoOpen>]
module StartExam = 
    [<CLIMutable>]
    type StartExamRequest =
        {
            [<JsonPropertyName("examId")>]
            ExamId : Guid
        }

    let validateStartExam req = async {
        return Ok req.ExamId
    }

    let toStartExamCommand examId =
        StartExam examId

    let startExamCommander = {
        Validate = validateStartExam
        ToCommand = toStartExamCommand
    }
