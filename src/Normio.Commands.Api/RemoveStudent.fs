module Normio.Commands.Api.RemoveStudent

open FSharp.Data
open Normio.Core.Commands
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

let validateRemoveStudent getExamByExamId (examId, studentId) = async {
    let! exam = getExamByExamId examId
    match exam with
    | Some _ -> return Choice1Of2 (examId, studentId)
    | _ -> return Choice2Of2 "Invalid Exam Id"
}

let removeStudentCommander getExamByExamId = {
    Validate = validateRemoveStudent getExamByExamId
    ToCommand = RemoveStudent
}
