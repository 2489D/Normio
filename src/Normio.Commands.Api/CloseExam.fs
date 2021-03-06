[<AutoOpen>]
module Normio.Commands.Api.CloseExam

open System
open System.Text.Json.Serialization
open Normio.Core.Commands
open Normio.Commands.Api.CommandHandlers

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
