module Normio.Web.Dev.CommandApiHandler

open System
open System.IO
open System.Text.Json.Serialization
open Microsoft.AspNetCore.Http
open FSharp.Control.Tasks.V2.ContextInsensitive

open Giraffe

open Normio.Persistence.EventStore
open Normio.Commands.Api
open Normio.Commands.Api.Compositions
open Normio.Persistence.FileSave
open Normio.Web.Dev.Hub

let commandApiHandler handler (eventStore : IEventStore) request : HttpHandler =
    fun (next: HttpFunc) (ctx : HttpContext) ->
        task {
            let eventHub = ctx.GetService<NormioEventWorker>()
            let! response = handler eventStore request
            match response with
            | Ok (state, events) ->
                do! eventStore.SaveEvents events
                eventHub.Trigger events
                return! json state next ctx
            | Error msg ->
                // TODO : status code
                return! (setStatusCode 400 >=> json msg) next ctx
        }

[<CLIMutable>]
type SubmissionUpload =
    {
        [<JsonPropertyName("examId")>]
        ExamId: Guid
        [<JsonPropertyName("studentId")>]
        StudentId: Guid
        [<JsonPropertyName("submissionId")>]
        SubmissionId: Guid
    }

[<CLIMutable>]
type QuestionUpload =
    {
        [<JsonPropertyName("examId")>]
        ExamId: Guid
        [<JsonPropertyName("hostId")>]
        HostId: Guid
        [<JsonPropertyName("questionId")>]
        QuestionId: Guid
    }

type UploadContext =
    | Submission of SubmissionUpload
    | Question of QuestionUpload

let fileUploadHandler uploadContext (fileSaver: IFileSaver) =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            match ctx.Request.HasFormContentType with
            | false ->
                return! RequestErrors.BAD_REQUEST "No forms are found" next ctx
            | true ->
                match uploadContext with
                | Submission upload ->
                    let file = ctx.Request.Form.Files.["submission"]
                    let extension = Path.GetExtension(file.Name)
                    use stream = file.OpenReadStream()
                    let uploadCtx = (upload.ExamId, upload.StudentId, upload.SubmissionId)
                    do! fileSaver.SaveSubmission uploadCtx extension stream.CopyTo
                | Question upload ->
                    let file = ctx.Request.Form.Files.["submission"]
                    let extension = Path.GetExtension(file.Name)
                    use stream = file.OpenReadStream()
                    printfn "%A" (upload.ExamId, upload.HostId, upload.QuestionId)
                    let uploadCtx = (upload.ExamId, upload.QuestionId)
                    do! fileSaver.SaveQuestion uploadCtx extension stream.CopyTo
                printfn "Form: %A" (ctx.Request.Form.Files.["submission"].Length)
                return! next ctx
        }

// Naming Convention: Resource names follows RPC style (example: Slack API)
let commandApi eventStore fileSaver =
    POST >=> subRoute "/api" (
        choose [
            route "/openExam" >=> bindJson<OpenExamRequest> (fun req -> commandApiHandler handleOpenExamRequest eventStore req)
            route "/startExam" >=> bindJson<StartExamRequest> (fun req -> commandApiHandler handleStartExamRequest eventStore req)
            route "/endExam" >=> bindJson<EndExamRequest> (fun req -> commandApiHandler handleEndExamRequest eventStore req)
            route "/closeExam" >=> bindJson<CloseExamRequest> (fun req -> commandApiHandler handleCloseExamRequest eventStore req)
            route "/addStudent" >=> bindJson<AddStudentRequest> (fun req -> commandApiHandler handleAddStudentRequest eventStore req)
            route "/removeStudent" >=> bindJson<RemoveStudentRequest> (fun req -> commandApiHandler handleRemoveStudentRequest eventStore req)
            route "/addHost" >=> bindJson<AddHostRequest> (fun req -> commandApiHandler handleAddHostRequest eventStore req)
            route "/removeHost" >=> bindJson<RemoveHostRequest> (fun req -> commandApiHandler handleRemoveHostRequest eventStore req)
            route "/changeTitle" >=> bindJson<ChangeTitleRequest> (fun req -> commandApiHandler handleChangeTitleRequest eventStore req)
            subRoute "/createSubmission" (
                choose [
                    route "/" >=> bindJson<CreateSubmissionRequest> (fun req -> commandApiHandler handleCreateSubmissionRequest eventStore req)
                    // TODO : validate upload information
                    route "/upload" >=> tryBindForm<SubmissionUpload> (fun err -> RequestErrors.BAD_REQUEST err) None (fun upload -> fileUploadHandler (Submission upload) fileSaver)
                ]
            )
            subRoute "/createQuestion" (
                choose [
                    route "/" >=> bindJson<CreateQuestionRequest> (fun req -> commandApiHandler handleCreateQuestionRequest eventStore req)
                    // TODO : validate upload information
                    route "/upload" >=> tryBindForm<QuestionUpload> (fun err -> RequestErrors.BAD_REQUEST err) None (fun upload -> fileUploadHandler (Question upload) fileSaver)
                ]
            )
            route "/sendMessage" >=> bindJson<SendMessageRequest> (fun req -> commandApiHandler handleSendMessageRequest eventStore req)
            route "/deleteQuestion" >=> bindJson<DeleteQuestionRequest> (fun req -> commandApiHandler handleDeleteQuestionRequest eventStore req)
        ]
    )
