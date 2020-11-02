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

// TODO : checking the examId exists in the readModel is not the responsibility of this validate function
let validateStartExam req = async {
    return Ok req.ExamId
}

let toStartExamCommand examId =
    StartExam examId

let startExamCommander = {
    Validate = validateStartExam
    ToCommand = toStartExamCommand
}
