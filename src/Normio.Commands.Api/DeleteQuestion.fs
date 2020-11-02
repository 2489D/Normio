[<AutoOpen>]
module Normio.Commands.Api.DeleteQuestion

open System
open System.Text.Json.Serialization
open Normio.Core.Commands
open Normio.Commands.Api.CommandHandlers

[<CLIMutable>]
type DeleteQuestionRequest =
    {
        [<JsonPropertyName("examId")>]
        ExamId : Guid
        [<JsonPropertyName("questionId")>]
        QuestionId : Guid
    }
        

let validateDeleteQuestion req = async {
    return Ok (req.ExamId, req.QuestionId)
}

let deleteQuestionCommander = {
    Validate = validateDeleteQuestion
    ToCommand = DeleteQuestion
}