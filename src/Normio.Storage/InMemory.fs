module Normio.Storage.InMemory

open System
open Normio.Storage.EventStore
open Normio.States
open Normio.Events

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
    | Some es -> Map.add examId (event :: es) store |> Ok
    | None -> FailToFindExam examId |> Error

let private saveEventsInner store events =
    List.fold (fun res evt ->
        match res with
        | Ok s -> saveEventInner s evt
        | Error _ -> res
    ) (Ok store) events

let private getEvents examId =
    match Map.tryFind examId eventStoreMap with
    | Some es -> Ok es
    | None -> FailToFindExam examId |> Error

let saveEvents events =
    match saveEventsInner eventStoreMap events with
    | Ok store ->
        eventStoreMap <- store
        Ok ()
    | Error err -> Error err
    |> async.Return

let getState examId =
    match getEvents examId with
    | Ok es ->
        List.fold apply (ExamIsClose None) es |> Ok
    | Error msg -> Error msg
    |> async.Return

let eventStoreInMemory = {
    GetState = getState
    SaveEvents = saveEvents
}
