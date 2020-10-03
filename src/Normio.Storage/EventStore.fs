module Normio.Storage.EventStore

open System
open Normio.States
open Normio.Events

type EventStoreError =
    | FailToFindExam of Guid

module EventStoreError =
    let toString = function
        | FailToFindExam examId -> sprintf "Fail to find the exam of %A" examId

type Err = EventStoreError
type EventStore = {
    GetState: Guid -> Async<State>
    SaveEvents: Event list -> Async<unit>
}
