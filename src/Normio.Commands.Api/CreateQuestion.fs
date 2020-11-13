[<AutoOpen>]
module Normio.Commands.Api.CreateQuestion

open System
open System.Text.Json.Serialization
open Normio.Core.Commands
open Normio.Commands.Api.CommandHandlers
open Normio.Core.Entities

[<CLIMutable>]
type CreateQuestionRequest =
    {
        [<JsonPropertyName("examId")>]
        ExamId : Guid
        [<JsonPropertyName("hostId")>]
        HostId : Guid
    }

let validateCreateQuestion req =
    async {
        return Ok (req.ExamId, req.HostId)
    }

let toCreateQuestionCommand (examId, hostId) =
    let question: Question =
        { Id = Guid.NewGuid()
          ExamId = examId
          HostId = hostId
          TimeStamp = DateTime.UtcNow }
    
    CreateQuestion (examId, question)

let createQuestionCommander = {
    Validate = validateCreateQuestion
    ToCommand = toCreateQuestionCommand
}


