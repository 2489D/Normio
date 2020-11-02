[<AutoOpen>]
module Normio.Commands.Api.CreateQuestion

open System
open System.Text.Json.Serialization
open Normio.Core.Commands
open Normio.Commands.Api.CommandHandlers

[<CLIMutable>]
type CreateQuestionRequest =
    {
        [<JsonPropertyName("examId")>]
        ExamId : Guid
    }

let validateCreateQuestion req = async {
    return Ok (req.ExamId)
}

let createQuestionCommander = {
    Validate = validateCreateQuestion
    ToCommand = fun examId -> CreateQuestion (examId, Guid.NewGuid())
}


