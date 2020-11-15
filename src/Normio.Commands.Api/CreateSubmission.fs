namespace Normio.Commands.Api

open System
open System.Text.Json.Serialization
open Normio.Core
open Normio.Core.Commands
open Normio.Commands.Api.CommandHandlers

[<AutoOpen>]
module CreateSubmission =
    [<CLIMutable>]
    type CreateSubmissionRequest =
        {
            [<JsonPropertyName("examId")>]
            ExamId : Guid
            [<JsonPropertyName("studentId")>]
            StudentId : Guid
            [<JsonPropertyName("title")>]
            Title : string
            [<JsonPropertyName("description")>]
            Description : string option
        }

    let validateCreateSubmission req = async {
        return Ok (req.ExamId, req.StudentId, req.Title, req.Description)
    }

    let toCreateSubmissionCommand (examId, studentId, title, desc) =
        let submission: Submission =
            { Id = Guid.NewGuid()
              ExamId = examId
              StudentId = studentId
              Title = title
              Description = desc
              CreatedDateTime = DateTime.UtcNow }
        CreateSubmission (examId, submission)
        

    let createSubmissionCommander = {
        Validate = validateCreateSubmission
        ToCommand = toCreateSubmissionCommand
    }
