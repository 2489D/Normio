namespace Normio.Timer

open System

[<AutoOpen>]
module Timer =
    // TODO : has to select what are really needed
    type ITimer =
        inherit IDisposable

        abstract SetTimer: time:DateTime -> task:Async<unit> -> Guid
        abstract TryGetTimer: timerId:Guid -> TimerData option
        abstract GetAllTimers: seq<TimerData>
        abstract DeleteTimer: timerId:Guid -> Async<unit>
        // TODO : how to update element in heap?
        // abstract UpdateTimer: Guid -> DateTime -> unit
        // abstract UpdateTask: Guid -> Async<unit> -> unit
