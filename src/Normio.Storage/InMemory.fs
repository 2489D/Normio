module Normio.Storage.InMemory

open System
open Normio.Core.Events
open Normio.Core.States
open Normio.Storage.Queries
open Normio.Storage.Projections
open Normio.Storage.EventStore
open Normio.Storage.Exams

let private getExamIdFromEvent = function
    | ExamOpened (id, _) -> id
    | ExamStarted id -> id
    | ExamEnded id -> id
    | ExamClosed id -> id
    | StudentEntered (id, _) -> id
    | StudentLeft (id, _) -> id
    | HostEntered (id, _) -> id
    | HostLeft (id, _) -> id
    | SubmissionCreated (id, _) -> id
    | QuestionCreated (id, _) -> id
    | QuestionDeleted (id, _) -> id
    | TitleChanged (id, _) -> id

type InMemoryEventStore() =
    let mutable eventStore: Map<Guid, Event list> = Map.empty

    let storeWithNewEvent store event =
        let examId = getExamIdFromEvent event
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

let inMemoryQueries: Queries = {
    Exam = examQueries
}

let inMemoryActions = {
    Exam = examActions
}
