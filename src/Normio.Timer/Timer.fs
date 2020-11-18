namespace Normio.Timer

open System
open Normio.Core.Commands

[<AutoOpen>]
module Timer =
    type ITimer =
        inherit IDisposable

        abstract SetTimer: command:Command -> time:DateTime -> Async<unit>
        abstract GetAllTimers: Async<seq<TimerData>>
        abstract DeleteTimer: command:Command -> Async<unit>
        abstract UpdateTimer: command:Command -> time:DateTime -> Async<unit>
