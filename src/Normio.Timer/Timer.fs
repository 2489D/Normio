namespace Normio.Timer

open System

[<AutoOpen>]
module Timer =
    // TODO : has to select what are really needed
    type ITimer =
        inherit IDisposable

        abstract SetTimer: time:DateTime -> task:Async<unit> -> Async<Guid>
        abstract TryGetTimer: timerId:Guid -> Async<TimerData option>
        abstract GetAllTimers: Async<seq<TimerData>>
        abstract DeleteTimer: timerId:Guid -> Async<unit>
        abstract UpdateTimer: timerId:Guid -> time:DateTime -> Async<unit>
