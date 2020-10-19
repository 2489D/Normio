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
    

