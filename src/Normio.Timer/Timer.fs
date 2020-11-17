namespace Normio.Timer

open System

[<AutoOpen>]
module Timer =
    // TODO : has to select what are really needed
    type ITimer =
        inherit IDisposable

        abstract SetTimer: TimerData -> Async<unit>
        abstract GetAllTimers: Async<seq<TimerData>>
        abstract DeleteTimer: TimerData -> Async<unit>
        abstract UpdateTimer: prev: TimerData -> newData: TimerData -> Async<unit>
