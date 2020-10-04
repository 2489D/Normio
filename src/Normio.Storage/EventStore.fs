module Normio.Storage.EventStore

open System
open Normio.States
open Normio.Events

type EventStore = {
    GetState: Guid -> Async<State>
    SaveEvents: Event list -> Async<unit>
}
