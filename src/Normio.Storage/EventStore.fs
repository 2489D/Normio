module Normio.Storage.EventStore

open System
open Normio.States
open Normio.Events

type EventStoreError =
    | FailToFindExam of Guid

type Err = EventStoreError
type EventStore = {
    GetState: Guid -> Async<Result<State, Err>>
    SaveEvents: Event list -> Async<Result<unit, Err>>
}