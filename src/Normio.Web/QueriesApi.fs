module Normio.Web.QueriesApi

open System.Text
open FSharp.Data
open Suave
open Suave.Operators
open Suave.Filters
open Suave.Successful
open Suave.RequestErrors

open Normio.Storage.Queries
open Normio.Web.JsonFormatter

[<Literal>]
let GetExamJson = """ {
    "getExam" : {
        "examId": "76cd4438-f93a-4420-8aca-1ee05f7f8063"
    }
}
"""

type GetExamRequest = JsonProvider<GetExamJson>

let (|GetExamRequest|_|) payload =
    try
        let req = GetExamRequest.Parse(payload).GetExam;
        req.ExamId |> Some
    with
    | _ -> None

let getExamByExamId examQuery (ctx: HttpContext) = async {
    let payload = ctx.request.rawForm |> Encoding.UTF8.GetString
    match payload with
    | GetExamRequest examId ->
        let! examReadModel = examQuery examId
        match examReadModel with
        | Some exam' ->
            return! OK (sprintf "%A" exam') ctx
        | None -> return! toErrorJson "Invalid Exam Id" ctx
    | _ -> return! toErrorJson "Invalid Payload" ctx
}

let queriesApi queries eventStore =
    GET >=> choose [
        path "/exams" >=> getExamByExamId queries.Exam.GetExamByExamId

    ]