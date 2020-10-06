module Normio.Commands.Api.AddStudent

open System
open FSharp.Data
open Normio.Core.Domain
open Normio.Core.States
open Normio.Core.Commands
open Normio.Storage.ReadModels
open Normio.Commands.Api.CommandHandlers

[<Literal>]
let AddStudentJson = """ {
"addStudent" : {
        "examId" : "2a964d85-f503-40a1-8014-2c8ee5ac4a49",
        "name" : "John"
    }
}
"""

type AddStudentRequest = JsonProvider<AddStudentJson>

let (|AddStudentRequest|_|) payload =
    try
        let req = AddStudentRequest.Parse(payload).AddStudent
        (req.ExamId, ({ Id = Guid.NewGuid(); Name = req.Name}: Student)) |> Some
    with
    | _ -> None

let validateAddStudent getState (req: Guid * Student) = async {
    let examId, student = req
    let! state = getState examId
    match state with
    | ExamIsWaiting _ -> return Choice1Of2 (examId, student)
    | _ -> return Choice2Of2 "Exam is not waiting"
}

let addStudentCommander getState = {
    Validate = validateAddStudent getState
    ToCommand = AddStudent
}
