namespace Normio.Persistence.EventStore

open System
open Normio.Core.Events
open Normio.Core.States

type IEventStore =
    abstract GetState: examId:Guid -> Async<State>
    abstract SaveEvents: event:Event list -> Async<unit>

