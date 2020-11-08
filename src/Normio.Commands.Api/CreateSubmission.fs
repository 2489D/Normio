[<AutoOpen>]
module Normio.Commands.Api.CreateSubmission

open System
open System.Text.Json.Serialization
open Normio.Core
open Normio.Core.Commands
open Normio.Commands.Api.CommandHandlers

[<CLIMutable>]
type CreateSubmissionRequest =
    {
        [<JsonPropertyName("examId")>]
        ExamId : Guid
        [<JsonPropertyName("studentId")>]
        StudentId : Guid
        [<JsonPropertyName("fileString")>]
        FileString : string
    }

let validateCreateSubmission req = async {
    match req.FileString |> FileString200.create with
    | Ok fileString -> return Ok (req.ExamId, req.StudentId, fileString)
    | Error err -> return err.ToString() |> Error
}

let toCreateSubmissionCommand (examId, studentId, fileString) =
    let submission: Submission =
        { Id = Guid.NewGuid()
          ExamId = examId
          StudentId = studentId
          File = {
              Id = Guid.NewGuid()
              FileName = fileString
          }
          TimeStamp = DateTime.UtcNow }
    CreateSubmission (examId, submission)
    

let createSubmissionCommander = {
    Validate = validateCreateSubmission
    ToCommand = toCreateSubmissionCommand
}
