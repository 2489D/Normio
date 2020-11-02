[<AutoOpen>]
module Normio.Commands.Api.AddStudent

open System
open System.Text.Json.Serialization
open Normio.Core.Domain
open Normio.Core.Commands
open Normio.Commands.Api.CommandHandlers

[<CLIMutable>]
type AddStudentRequest =
    {
        [<JsonPropertyName("examId")>]
        ExamId : Guid
        [<JsonPropertyName("name")>]
        Name : string
    }

let validateAddStudent req = async {
    match req.Name |> UserName40.create with
    | Ok name40 ->
        return Ok (req.ExamId, name40)
    | Error err -> return err.ToString() |> Error
}

let toAddStudentCommand (examId, name) =
    let student: Student =
        { Id = Guid.NewGuid()
          Name = name }
    AddStudent (examId, student)

let addStudentCommander = {
    Validate = validateAddStudent
    ToCommand = toAddStudentCommand
}
