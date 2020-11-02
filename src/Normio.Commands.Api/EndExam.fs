[<AutoOpen>]
module Normio.Commands.Api.EndExam

open System
open System.Text.Json.Serialization
open Normio.Core.Commands
open Normio.Commands.Api.CommandHandlers

[<CLIMutable>]
type EndExamRequest =
    {
        [<JsonPropertyName("examId")>]
        ExamId : Guid
    }

let validateEndExam req = async {
    return req.ExamId |> Ok
}

let endExamCommander = {
    Validate = validateEndExam
    ToCommand = EndExam
}
