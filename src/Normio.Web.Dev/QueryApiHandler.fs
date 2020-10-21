module Normio.Web.Dev.QueryApiHandler

open System.IO
open Microsoft.AspNetCore.Http

open FSharp.Data
open FSharp.Control.Tasks.V2.ContextInsensitive

open Giraffe

open Normio.Persistence.EventStore
open Normio.Persistence.Queries

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

let getExamByExamId examQuery =
    fun (next : HttpFunc) (context : HttpContext) -> task {
        use stream = new StreamReader(context.Request.Body);
        let! payload = stream.ReadToEndAsync();
        match payload with
        | GetExamRequest examId ->
            let! examReadModel = examQuery examId
            match examReadModel with
            | Some exam' -> return! json exam' next context
            | None -> return! text "invalid exam id" next context
        | _ -> return! text "invalid payload" next context
    }

let queriesApi queries (eventStore : IEventStore) =
    GET >=> choose [
        route "/exams" >=> getExamByExamId queries.Exam.GetExamByExamId
    ]