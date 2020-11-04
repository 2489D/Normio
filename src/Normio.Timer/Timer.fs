namespace Normio.Timer.Timer

open System

open Normio.Timer.Domain

// TODO : has to select what are really needed
// TODO : error handling
type ITimer =
    abstract SetTimer: DateTime -> Async<unit> -> Guid
    abstract GetTimer: Guid -> TimerData option
    abstract GetAllTimers: seq<TimerData>
    // TODO : how to update element in heap?
    // abstract UpdateTime: Guid -> DateTime -> unit
    // abstract UpdateTask: Guid -> Async<unit> -> unit
