module Normio.Persistence.EventStore.Cosmos

open System
open System.Text.Json.Serialization
open FSharp.Control
open FSharp.CosmosDb
open Normio.Core.Events
open Normio.Core.States
open Normio.Persistence.EventStore

[<CLIMutable>]
type EventStored = {
    [<JsonPropertyName("id")>]
    Id : Guid // unique identifier for an event data
    [<PartitionKey>]
    EventId : Guid
    Event : Event
}

type CosmosEventStore(connString: string) =
    let getConn =
        connString
        |> Cosmos.fromConnectionString
        |> Cosmos.database "EventStore"
        |> Cosmos.container "eventContainer"
        
    let getEvents =
        fun (examId: Guid) ->
            getConn
            |> Cosmos.query "SELECT * FROM e WHERE e.EventId = @Id"
            |> Cosmos.parameters [
                "@Id", box examId
            ]
            |> Cosmos.execAsync<EventStored>

    let getState =
        fun examId -> async {
                let! state = getEvents examId
                             |> AsyncSeq.map (fun stored -> stored.Event)
                             |> AsyncSeq.fold apply (ExamIsClose None)
                return state
            }

    let saveEvents =
        fun (events: Event list) -> async {
            let eventsToBeStored =
                events
                |> List.map (fun event ->
                    { Id = Guid.NewGuid()
                      EventId = getExamIdFromEvent event
                      Event = event })
            getConn
            |> Cosmos.insertMany eventsToBeStored
            |> Cosmos.execAsync
            |> ignore
        }
    
    interface IEventStore with
        member this.GetState examId = getState examId
        member this.SaveEvents events = saveEvents events

type Init =
    | Uninitialized
    | Initialized of CosmosEventStore

let mutable private cosmosEventStoreInstance: Init = Uninitialized

let rec cosmosEventStore connString =
    match cosmosEventStoreInstance with
    | Initialized eventStore -> eventStore :> IEventStore
    | Uninitialized ->
        cosmosEventStoreInstance <- Initialized (CosmosEventStore connString)
        cosmosEventStore connString
        
