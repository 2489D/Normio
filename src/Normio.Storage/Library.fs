module Normio.Storage

open System
open Normio.Domain
open Normio.States
open Normio.Events

type EventStore = {
    GetState: Guid -> Async<Result<State, string>>
    SaveEvents: Event list -> Async<Result<unit, string>>
}
(*
let getExamIdFromState = function
    | ExamIsClose id -> id
    | ExamIsWaiting exam -> Some exam.Id
    | ExamIsRunning exam -> Some exam.Id
    | ExamIsFinished exam -> Some exam.Id

module EventStore =
    let createStorage (): EventStore = Map.empty
    let tryStoreEvent (store: EventStore) state event =
        match getExamIdFromState state with
        | Some examId ->
            let events = Map.tryFind examId store
            match events with
            | Some es -> Map.add examId (event :: es) store |> Ok
            | None -> "Fail to find the exam" |> Error
        | None -> "Unable to store the event" |> Error
    
    let tryStoreEvents (store: EventStore) state events =
        List.fold (fun rs e ->
        match rs with
        | Ok s ->
            match tryStoreEvent s state e with
            | Ok newStore -> newStore |> Ok
            | Error msg -> msg |> Error
        | Error msg -> msg |> Error
        ) (Ok store) events
        
    let tryGetEvents (store: EventStore) examId =
        match Map.tryFind examId store with
        | Some es -> Ok es
        | None -> "Invalid exam id" |> Error
    
    let tryGetState (store: EventStore) examId =
        match tryGetEvents store examId with
        | Ok es ->
            List.fold apply (ExamIsClose None) es |> Ok
        | Error msg -> Error msg
*)