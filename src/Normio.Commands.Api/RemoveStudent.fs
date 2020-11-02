[<AutoOpen>]
module Normio.Commands.Api.RemoveStudent

open System
open System.Text.Json.Serialization
open Normio.Core.Commands
open Normio.Commands.Api.CommandHandlers

[<CLIMutable>]
type RemoveStudentRequest =
    {
        [<JsonPropertyName("examId")>]
        ExamId : Guid
        [<JsonPropertyName("studentId")>]
        StudentId : Guid
    }

let validateRemoveStudent req = async {
    return Ok (req.ExamId, req.StudentId)
}

let removeStudentCommander = {
    Validate = validateRemoveStudent
    ToCommand = RemoveStudent
}
