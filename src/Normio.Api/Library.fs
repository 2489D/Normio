module Normio.Api

open System
open Normio.Domain
open Normio.CommandHandlers
open Normio.Storage

/// 1. Produce events with the given command and the state
/// 2. Save the events to EventStore
/// 3. Returns the new store
let commandHandler state command (store: EventStore) =
    match execute state command with
    | Ok es -> 
        EventStore.tryStoreEvents store state es
    | Error msg ->
        Error msg
