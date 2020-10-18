module Normio.Storage.EventStore.InMemory

open System
open Normio.Core.Events
open Normio.Core.States
open Normio.Storage.EventStore

type InMemoryEventStore() =
    let mutable eventStore: Map<Guid, Event list> = Map.empty

    let storeWithNewEvent store event =
        let examId = Helper.getExamIdFromEvent event
        match Map.tryFind examId store with
        | Some es -> Map.add examId (event :: es) store
        | None -> Map.add examId [event] store
    
    let storeWithNewEvents store events =
        events |> List.fold storeWithNewEvent store
    
    let getEvents store examId =
        match Map.tryFind examId store with
        | Some es -> es |> List.rev
        | None -> []
    
    member this.GetState examId = async {
        return getEvents eventStore examId
        |> List.fold apply (ExamIsClose None)
    }

    member this.SaveEvents events = async {
        let newStore = storeWithNewEvents eventStore events
        eventStore <- newStore
    }

let private eventStoreObj = InMemoryEventStore()


let inMemoryEventStore = {
    GetState = eventStoreObj.GetState
    SaveEvents = eventStoreObj.SaveEvents
}
