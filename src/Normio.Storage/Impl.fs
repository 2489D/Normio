module Normio.Storage.InMemory

open System
open Normio.Events
open System.Collections.Generic

let eventStoreDict = new Dictionary<Guid, Event list>()

let getState = ...
let getEvents examId =
    let (_, events) = eventStoreDict.TryGetValue(examId)
    events
let saveEvent = ...
let saveEvents = ...

let eventStore = {
    GetState = getState
    SaveEvents = saveEvents
}
