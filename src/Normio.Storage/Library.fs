module Normio.Storage

open System
open Normio.Domain
open Normio.States
open Normio.Events

type EventStore = {
    GetState: Guid -> Async<Result<State, string>>
    SaveEvents: Event list -> Async<Result<unit, string>>
}