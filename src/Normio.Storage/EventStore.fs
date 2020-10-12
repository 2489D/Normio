module Normio.Storage.EventStore

open System
open Normio.Core.Events
open Normio.Core.States

type EventStore = {
    GetState: Guid -> Async<State>
    SaveEvents: Event list -> Async<unit>
}
