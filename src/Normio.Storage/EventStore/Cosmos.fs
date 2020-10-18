module Normio.Storage.EventStore.Cosmos

open System
open FSharp.Control
open FSharp.CosmosDb
open Normio.Core.Domain
open Normio.Core.Events
open Normio.Core.States
open Normio.Storage.EventStore
open Normio.Storage.EventStore.EventStoredTypes

type Event with
    member this.Name =
        match this with
        | ExamOpened _ -> "ExamOpened"
        | ExamStarted _ -> "ExamStarted"
        | ExamEnded _ -> "ExamEnded"
        | ExamClosed _ -> "ExamClosed"
        | StudentEntered _ -> "StudentEntered"
        | StudentLeft _ -> "StudentLeft"
        | HostEntered _ -> "HostEntered"
        | HostLeft _ -> "HostLeft"
        | SubmissionCreated _ -> "SubmissionCreated"
        | QuestionCreated _ -> "QuestionCreated"
        | QuestionDeleted _ -> "QuestionDeleted"
        | TitleChanged _ -> "TitleChanged"
    
    member this.Data =
        match this with
        | ExamOpened (examId, title) ->
            sprintf """{"title" : "%s"}""" title.Value
        | ExamStarted examId -> null
        | ExamEnded examId -> null
        | ExamClosed examId -> null
        | StudentEntered (examId, student) ->
            sprintf """{"studentId" : "%s", "name" : "%s"}""" (student.Id.ToString()) student.Name.Value
        | StudentLeft (examId, studentId) ->
            sprintf """{"studentId" : "%s"}""" (studentId.ToString())
        | HostEntered (examId, host) ->
            sprintf """{"hostId" : "%s", "name" : "%s"}""" (host.Id.ToString()) host.Name.Value
        | HostLeft (examId, hostId) ->
            sprintf """{"hostId" : "%s"}""" (hostId.ToString())
        | SubmissionCreated (examId, subm) ->
            sprintf """{
                    "submissionId" : "%s",
                    "examId" : "%s",
                    "student" : {
                        "studentId" : "%s",
                        "name" : "%s"
                    },
                    "file" : {
                        "fileId" : "%s",
                        "name" : "%s"
                    }
                }"""
                (subm.Id.ToString())
                (subm.ExamId.ToString())
                (subm.Student.Id.ToString())
                (subm.Student.Name.Value)
                (subm.File.Id.ToString())
                (subm.File.FileName.Value)
        | QuestionCreated (examId, file) ->
            sprintf """{
                "fileId" : "%s",
                "name" : "%s"
            }""" (file.Id.ToString()) (file.FileName.Value)
        | QuestionDeleted (examId, fileId) ->
            sprintf """{
                "fileId" : "%s"
            }""" (fileId.ToString())
        | TitleChanged (examId, title) ->
            sprintf """{ "title" : "%s" }""" title.Value
        

let private getConn connString =
    connString
    |> Cosmos.fromConnectionString
    |> Cosmos.database "EventStore"
    |> Cosmos.container "eventContainer"
    
let private getEvents connString =
    fun (examId: Guid) ->
        getConn connString
        |> Cosmos.query "SELECT * FROM e WHERE e.examId = @Id"
        |> Cosmos.parameters [
            "@Id", box examId
        ]
        |> Cosmos.execAsync<EventStored>

let private getEventFromStored eventStored =
    match eventStored with
    | ExamOpenedStored event -> event |> Ok
    | ExamStartedStored event -> event |> Ok
    | ExamEndedStored event -> event |> Ok
    | ExamClosedStored event -> event |> Ok
    | StudentEnteredStored event -> event |> Ok
    | StudentLeftStored event -> event |> Ok
    | HostEnteredStored event -> event |> Ok
    | HostLeftStored event -> event |> Ok
    | SubmissionCreatedStored event -> event |> Ok
    | QuestionCreatedStored event -> event |> Ok
    | QuestionDeletedStored event -> event |> Ok
    | TitleChangedStored event -> event |> Ok
    | _ -> "Invalid event stored" |> Error
    

let private mapEventToStoredType (event: Event) =
    { Id = Guid.NewGuid()
      Event = event.Name
      ExamId = Helper.getExamIdFromEvent event
      Data = event.Data }


let private getState connString =
    let folder state event =
        match event with
        | Ok event' ->
            match state with
            | Ok state' -> apply state' event' |> Ok
            | Error err -> Error err
        | Error err -> Error err
    fun examId -> async {
            let! state = getEvents connString examId
                        |> AsyncSeq.map getEventFromStored
                        |> AsyncSeq.fold folder (Ok (ExamIsClose None))
            match state with
            | Ok state' -> return state'
            | _ -> return (ExamIsClose None)
        }

let private saveEvents connString =
    fun events -> async {
        let eventsStored = List.map mapEventToStoredType events
        getConn connString
        |> Cosmos.insertMany eventsStored
        |> Cosmos.execAsync
        |> ignore
    }

let cosmosEventStore connString = {
    GetState = getState connString
    SaveEvents = saveEvents connString
}



