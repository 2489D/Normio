module Normio.Web.Dev.CommandApiHandler

open Microsoft.AspNetCore.Http
open FSharp.Control.Tasks.V2.ContextInsensitive

open Giraffe

open Normio.Persistence.EventStore
open Normio.Commands.Api
open Normio.Web.Dev.Hub

// TODO : status code
let commandApiHandler handler (eventStore : IEventStore) request : HttpHandler =
    fun (next: HttpFunc) (ctx : HttpContext) -> task {
        let eventHub = ctx.GetService<NormioEventWorker>()
        let! response = handler eventStore request
        match response with
        | Ok (state, events) ->
            do! eventStore.SaveEvents events
            eventHub.Trigger events
            return! json state next ctx
        | Error msg ->
            return! (setStatusCode 404 >=> json msg) next ctx
    }

let commandApi eventStore =
    POST >=> choose [
        route "/openExam" >=> bindJson<OpenExamRequest> (fun req -> commandApiHandler handleOpenExamRequest eventStore req)
        route "/startExam" >=> bindJson<StartExamRequest> (fun req -> commandApiHandler handleStartExamRequest eventStore req)
        route "/endExam" >=> bindJson<EndExamRequest> (fun req -> commandApiHandler handleEndExamRequest eventStore req)
        route "/closeExam" >=> bindJson<CloseExamRequest> (fun req -> commandApiHandler handleCloseExamRequest eventStore req)
        route "/addStudent" >=> bindJson<AddStudentRequest> (fun req -> commandApiHandler handleAddStudentRequest eventStore req)
        route "/removeStudent" >=> bindJson<RemoveStudentRequest> (fun req -> commandApiHandler handleRemoveStudentRequest eventStore req)
        route "/addHost" >=> bindJson<AddHostRequest> (fun req -> commandApiHandler handleAddHostRequest eventStore req)
        route "/removeHost" >=> bindJson<RemoveHostRequest> (fun req -> commandApiHandler handleRemoveHostRequest eventStore req)
        route "/createSubmission" >=> bindJson<CreateSubmissionRequest> (fun req -> commandApiHandler handleCreateSubmissionRequest eventStore req)
        route "/createQuestion" >=> bindJson<CreateQuestionRequest> (fun req -> commandApiHandler handleCreateQuestionRequest eventStore req)
        route "/deleteQuestion" >=> bindJson<DeleteQuestionRequest> (fun req -> commandApiHandler handleDeleteQuestionRequest eventStore req)
        route "/changeTitle" >=> bindJson<ChangeTitleRequest> (fun req -> commandApiHandler handleChangeTitleRequest eventStore req)
    ]

