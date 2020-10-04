module Normio.Storage.InMemory

open System
open Normio.Events
open Normio.States
open Normio.Queries
open Normio.Storage.EventStore
open Normio.Storage.Exams

let getExamIdFromEvent = function
    | ExamOpened (id, _) -> id
    | ExamStarted id -> id
    | ExamEnded id -> id
    | ExamClosed id -> id
    | StudentEntered (id, _) -> id
    | StudentLeft (id, _) -> id
    | HostEntered (id, _) -> id
    | HostLeft (id, _) -> id
    | QuestionCreated (id, _) -> id
    | QuestionDeleted (id, _) -> id
    | TitleChanged (id, _) -> id

let mutable eventStoreMap: Map<Guid, Event list> = Map.empty

let private saveEventInner store event =
    let examId = getExamIdFromEvent event
    match Map.tryFind examId store with
    | Some es -> Map.add examId (event :: es) store
    | None -> Map.add examId [event] store

let private saveEventsInner store events =
    events |> List.fold saveEventInner store

let private getEvents examId =
    match Map.tryFind examId eventStoreMap with
    | Some es -> es
    | None -> []

let saveEvents events = async {
    let newStore =  saveEventsInner eventStoreMap events
    eventStoreMap <- newStore
}

let getState examId =
    getEvents examId
    |> List.fold apply (ExamIsClose None)
    |> async.Return

let inMemoryEventStore = {
    GetState = getState
    SaveEvents = saveEvents
}

let inMemoryQueries = {
    Exam = examQueries
}

let queryInMemory = {
    Exam = examQueryInMemory
}
