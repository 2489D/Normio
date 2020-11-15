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

// TODO: elaborate read model
// FIXME
let getExamByExamId examQuery =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            match ctx.TryBindQueryString<GetExamRequest>() with
            | Ok getExam -> 
                let! examReadModel = examQuery getExam.ExamId
                match examReadModel with
                | Some exam -> return! (json exam) next ctx
                | None -> return! RequestErrors.NOT_FOUND "Exam does not exist" next ctx
            | Error _ -> return! RequestErrors.BAD_REQUEST "Exam Id should be a GUID" next ctx
        }

let queriesApi queries =
    GET >=> choose [
        route "/exam" >=> getExamByExamId queries.Exam.GetExamByExamId
    ]