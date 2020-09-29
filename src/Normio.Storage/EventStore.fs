module Normio.Storage.EventStore

open System
open Normio.States
open Normio.Events

type EventStore = {
    GetState: Guid -> Async<Result<State, string>>
    SaveEvents: Event list -> Async<Result<unit, string>>
}