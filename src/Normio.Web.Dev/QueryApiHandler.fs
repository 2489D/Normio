module Normio.Web.Dev.QueryApiHandler

open System
open System.Text.Json.Serialization
open Microsoft.AspNetCore.Http
open FSharp.Control.Tasks.V2.ContextInsensitive

open Giraffe

open Normio.Persistence.Queries
open Normio.Persistence.FileSave

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

[<CLIMutable>]
type GetQuestionRequest =
    {
        [<JsonPropertyName("examId")>]
        ExamId : Guid
        [<JsonPropertyName("questionId")>]
        QuestionId : Guid
    }

let fileCallback stream =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            return! ctx.WriteStreamAsync false stream None None
        }

let ifQuestionNotFound ((examId, questionId): Guid * Guid) =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            return! RequestErrors.NOT_FOUND (sprintf "No Question: %A in exam: %A" questionId examId) next ctx
        }

let getQuestion (fileGetter: IFileGetter) =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            match ctx.TryBindQueryString<GetQuestionRequest>() with
            | Ok getQuestion ->
                let ids = (getQuestion.ExamId, getQuestion.QuestionId)
                let! res = fileGetter.GetQuestion ids fileCallback ifQuestionNotFound
                return! res next ctx
            | Error _ -> return! RequestErrors.BAD_REQUEST "Exam and Question Id should be GUID" next ctx
        }

[<CLIMutable>]
type GetSubmissionRequest =
    {
        [<JsonPropertyName("examId")>]
        ExamId : Guid
        [<JsonPropertyName("studentId")>]
        StudentId : Guid
        [<JsonPropertyName("submissionId")>]
        SubmissionId : Guid
    }

let ifSubmissionNotFound ((examId, studentId, submissionId): Guid * Guid * Guid) =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            return! RequestErrors.NOT_FOUND (sprintf "No Submission: %A in Student: %A in exam: %A" submissionId studentId examId) next ctx
        }

let getSubmission (fileGetter: IFileGetter) =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            match ctx.TryBindQueryString<GetSubmissionRequest>() with
            | Ok getSubmission ->
                let ids = (getSubmission.ExamId, getSubmission.StudentId, getSubmission.SubmissionId)
                let! res = fileGetter.GetSubmission ids fileCallback ifSubmissionNotFound
                return! res next ctx
            | Error _ -> return! RequestErrors.BAD_REQUEST "Exam, Student and Submission Id should be GUID" next ctx
        }


let queriesApi queries fileGetter =
    GET >=> choose [
        route "/exam" >=> getExamByExamId queries.Exam.GetExamByExamId
        route "/Question" >=> getQuestion fileGetter
        route "/Submission" >=> getSubmission fileGetter
    ]
