namespace Normio.Commands.Api

open System
open System.Text.Json.Serialization
open Normio.Core
open Normio.Core.Commands

[<AutoOpen>]
module LetStudentIn =
    [<CLIMutable>]
    type LetStudentInRequest =
        { [<JsonPropertyName("examId")>]
          ExamId: Guid
          [<JsonPropertyName("studentId")>]
          StudentId: Guid
          [<JsonPropertyName("name")>]
          Name: string }

    let validateLetStudentInRequest req =
        async {
            match req.Name |> UserName40.Create with
            | Ok name -> return Ok (req.ExamId, req.StudentId, name)
            | Error err -> return Error <| err.ToString()
        }

    let toLetStudentInCommand (examId, studentId, studentName) =
        let student: Student = { Id = studentId; Name = studentName }
        LetStudentIn(examId, student)

    let letStudentInCommander =
        { Validate = validateLetStudentInRequest
          ToCommand = toLetStudentInCommand }
