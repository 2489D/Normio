module Normio.Commands.Api.RemoveStudent

open System
open FSharp.Data
open Normio.Core.States
open Normio.Core.Commands
open Normio.Storage.ReadModels
open Normio.Commands.Api.CommandHandlers

[<Literal>]
let RemoveStudentJson = """ {
"removeStudent" : {
        "examId" : "2a964d85-f503-40a1-8014-2c8ee5ac4a49",
        "studentId" : "2a964d85-f503-40a1-8014-2c8ee5ac4a49"
    }
}
"""

type RemoveStudentRequest = JsonProvider<RemoveStudentJson>

let (|RemoveStudentRequest|_|) payload =
    try
        let req = RemoveStudentRequest.Parse(payload).RemoveStudent
        (req.ExamId, req.StudentId) |> Some
    with
    | _ -> None

let validateRemoveStudent getState (req: Guid * Guid) = async {
    let examId, studentId = req
    let! state = getState examId
    match state with
    | ExamIsWaiting exam ->
            if exam.Students |> Map.containsKey studentId
            then return Choice1Of2 (examId, studentId)
            else return Choice2Of2 "Invalid Student Id"
    | _ -> return Choice2Of2 "Exam is not waiting"
}

let removeStudentCommander getState = {
    Validate = validateRemoveStudent getState
    ToCommand = RemoveStudent
}
