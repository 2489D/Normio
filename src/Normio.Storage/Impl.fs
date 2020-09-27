module Normio.Storage.InMemory

open System
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

let saveEvent store event =
    let examId = getExamIdFromEvent event
    match Map.tryFind examId store with
    | Some es -> Map.add examId (event :: es) store |> Ok
    | None -> "Fail to find the exam" |> Error

let inSaveEvents store events =
    List.fold (fun rs e ->
    match rs with
    | Ok s ->
        match saveEvent s e with
        | Ok newStore -> newStore |> Ok
        | Error msg -> msg |> Error
    | Error _ -> rs
    ) (Ok store) events

let saveEvents events =
    match inSaveEvents eventStoreMap events with
    | Ok store ->
        eventStoreMap <- store
        () |> Ok
    | Error msg -> Error msg
    |> async.Return

let getEvents examId =
    match Map.tryFind examId eventStoreMap with
    | Some es -> Ok es
    | None -> "Invalid exam id" |> Error

let getState examId =
    match getEvents examId with
    | Ok es ->
        List.fold apply (ExamIsClose None) es |> Ok
    | Error msg -> Error msg
    |> async.Return

let eventStore = {
    GetState = getState
    SaveEvents = saveEvents
}
