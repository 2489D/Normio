[<AutoOpen>]
module Normio.Commands.Api.StartExam

open System
open System.Text.Json.Serialization
open Normio.Core.Commands
open Normio.Commands.Api.CommandHandlers

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
