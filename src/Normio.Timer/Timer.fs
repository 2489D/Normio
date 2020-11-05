namespace Normio.Timer.Timer

open System
open Normio.Timer.Domain
open Normio.Timer.Errors

[<AutoOpen>]
module Timer =
    // TODO : has to select what are really needed
    type ITimer =
        inherit IDisposable

        abstract SetTimer: DateTime -> Async<unit> -> Result<Guid, TimerError>
        abstract GetTimer: Guid -> TimerData option
        abstract GetAllTimers: seq<TimerData>
        // TODO : how to update element in heap?
        // abstract UpdateTime: Guid -> DateTime -> unit
        // abstract UpdateTask: Guid -> Async<unit> -> unit
