[<AutoOpen>]
module Normio.Commands.Api.CreateSubmission

open System
open System.Text.Json.Serialization
open Normio.Core.Domain
open Normio.Core.Commands
open Normio.Commands.Api.CommandHandlers

[<CLIMutable>]
type CreateSubmissionRequest =
    {
        [<JsonPropertyName("examId")>]
        ExamId : Guid
        [<JsonPropertyName("studentId")>]
        StudentId : Guid
    }

let validateCreateSubmission req = async {
    return Ok (req.ExamId, req.StudentId)
}

let toCreateSubmissionCommand (examId, studentId) =
    let submission: Submission =
        { Id = Guid.NewGuid()
          ExamId = examId
          StudentId = studentId
          TimeStamp = DateTime.UtcNow }
    CreateSubmission (examId, submission)
    

let createSubmissionCommander = {
    Validate = validateCreateSubmission
    ToCommand = toCreateSubmissionCommand
}
