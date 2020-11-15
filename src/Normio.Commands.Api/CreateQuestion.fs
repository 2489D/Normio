namespace Normio.Commands.Api

open System
open System.Text.Json.Serialization
open Normio.Core.Commands
open Normio.Core.Entities
open Normio.Commands.Api.CommandHandlers

[<AutoOpen>]
module CreateQuestion =
    [<CLIMutable>]
    type CreateQuestionRequest =
        {
            [<JsonPropertyName("examId")>]
            ExamId : Guid
            [<JsonPropertyName("hostId")>]
            HostId : Guid
            [<JsonPropertyName("title")>]
            Title : string
            [<JsonPropertyName("description")>]
            Description : string option
        }

    let validateCreateQuestion req =
        async {
            return Ok (req.ExamId, req.HostId, req.Title, req.Description)
        }

    let toCreateQuestionCommand (examId, hostId, title, desc) =
        let question: Question =
            { Id = Guid.NewGuid()
              ExamId = examId
              HostId = hostId
              Title = title
              Description = desc
              CreatedDateTime = DateTime.UtcNow }
        
        CreateQuestion (examId, question)

    let createQuestionCommander = {
        Validate = validateCreateQuestion
        ToCommand = toCreateQuestionCommand
    }
