module Normio.Web.Dev.QueryApiHandler

open System
open System.Text.Json.Serialization
open Microsoft.AspNetCore.Http
open FSharp.Control.Tasks.V2.ContextInsensitive

open Giraffe

open Normio.Persistence.Queries

[<CLIMutable>]
type GetExamRequest =
    {
        [<JsonPropertyName("examId")>]
        ExamId : Guid
    }

let getExamByExamId examQuery req =
    fun (next : HttpFunc) (context : HttpContext) ->
        task {
            let! examReadModel = examQuery req.ExamId
            match examReadModel with
            | Some exam -> return! (req.ToString() |> text) next context
            | None -> return! text "Exam does not exist" next context
        }

let queriesApi queries =
    GET >=> choose [
        route "/exams" >=> bindJson<GetExamRequest> (fun req -> getExamByExamId queries.Exam.GetExamByExamId req)
    ]