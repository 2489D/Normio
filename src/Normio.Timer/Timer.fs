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
        abstract UpdateTimer: timerId:Guid -> time:DateTime -> Async<unit>
