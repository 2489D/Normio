namespace Normio.Commands.Api

open System
open System.Text.Json.Serialization

open Normio.Core
open Normio.Core.Commands

[<AutoOpen>]
module LetStudentOut =
    [<CLIMutable>]
    type LetStudentOutRequest =
        { [<JsonPropertyName("examId")>]
          ExamId: Guid
          [<JsonPropertyName("studentId")>]
          StudentId: Guid }

    let validateLetStudentOutRequest req =
        async { return Ok(req.ExamId, req.StudentId) }

    let toLetStudentOutCommand (examId, studentId) = LetStudentOut(examId, studentId)

    let letStudentOutCommander =
        { Validate = validateLetStudentOutRequest
          ToCommand = toLetStudentOutCommand }
