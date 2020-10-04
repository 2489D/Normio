module StartExam

open FSharp.Data
open Commands
open ReadModels
open CommandHandlers

[<Literal>]
let StartExamJson = """ {
"startExam" : {
    "examId" : "2a964d85-f503-40a1-8014-2c8ee5ac4a49"
    }
}
"""

type StartExamRequest = JsonProvider<StartExamJson>

let (|StartExamRequest|_|) payload =
    try
        let req = StartExamRequest.Parse(payload).StartExam
        req.ExamId |> Some
    with
    | _ -> None

let validateStartExam getExam examId = async {
    let! exam = getExam examId
    match exam with
    | Some exam ->
        match exam.Status with
        | BeforeExam -> return Ok examId
        | _ -> return Error "Exam already started"
    | None -> return Error "Invalid Exam Id"
}

let startExamCommander getExam = {
    Validate = validateStartExam getExam
    ToCommand = StartExam
}
