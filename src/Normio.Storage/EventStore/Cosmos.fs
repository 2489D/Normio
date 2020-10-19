module Normio.Storage.EventStore.Cosmos

open System
open System.Text.Json.Serialization
open FSharp.Control
open FSharp.CosmosDb
open Normio.Core.Domain
open Normio.Core.Events
open Normio.Core.States
open Normio.Storage.EventStore

[<CLIMutable>]
type EventStored = {
    [<JsonPropertyName("id")>]
    Id : Guid // unique identifier for an event data
    [<PartitionKey>]
    EventId : Guid
    Event : Event
}

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

//let private getEventFromStored eventStored =
//    match eventStored with
//    | ExamOpenedStored event -> event |> Ok
//    | ExamStartedStored event -> event |> Ok
//    | ExamEndedStored event -> event |> Ok
//    | ExamClosedStored event -> event |> Ok
//    | StudentEnteredStored event -> event |> Ok
//    | StudentLeftStored event -> event |> Ok
//    | HostEnteredStored event -> event |> Ok
//    | HostLeftStored event -> event |> Ok
//    | SubmissionCreatedStored event -> event |> Ok
//    | QuestionCreatedStored event -> event |> Ok
//    | QuestionDeletedStored event -> event |> Ok
//    | TitleChangedStored event -> event |> Ok
//    | _ -> "Invalid event stored" |> Error
//    
//
let private getConn connString =
    connString
    |> Cosmos.fromConnectionString
    |> Cosmos.database "EventStore"
    |> Cosmos.container "eventContainer"
    
let private getEvents connString =
    fun (examId: Guid) ->
        getConn connString
        |> Cosmos.query "SELECT * FROM e WHERE e.EventId = @Id"
        |> Cosmos.parameters [
            "@Id", box examId
        ]
        |> Cosmos.execAsync<EventStored>

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
                         |> AsyncSeq.map (fun stored -> stored.Event)
                         |> AsyncSeq.fold apply (ExamIsClose None)
            return state
        }

let private saveEvents connString =
    fun (events: Event list) -> async {
        let eventsToBeStored =
            events
            |> List.map (fun event ->
                { Id = Guid.NewGuid()
                  EventId = getExamIdFromEvent event
                  Event = event })
        getConn connString
        |> Cosmos.insertMany eventsToBeStored
        |> Cosmos.execAsync
        |> ignore
    }

let cosmosEventStore connString =
    { new IEventStore with
        member this.GetState examId = getState connString examId
        member this.SaveEvents events = saveEvents connString events }
    

