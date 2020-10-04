module Normio.Storage.EventStore

open System
open Normio.Core.States
open Normio.Core.Events

type EventStore = {
    GetState: Guid -> Async<State>
    SaveEvents: Event list -> Async<unit>
}
