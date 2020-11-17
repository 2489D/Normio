namespace Normio.Timer

open System
open System.Runtime.Serialization
open FSharpx.Collections
open Normio.Core.Commands

[<AutoOpen>]
[<KnownType("timer")>]
module InMemory =
    type private InMemoryTimer(millisec, postCommand: Command -> Async<unit>) =
        let minHeap = false
        let mutable timerStore: Heap<TimerData> = Heap.empty minHeap

        let checker = new Timers.Timer(millisec)

        let handler (e: Timers.ElapsedEventArgs) =
            let rec loop ts tl =
                match ts with
                | Heap.Cons(h, t) ->
                    if h.Time <= e.SignalTime then
                        loop t (h.TaskCommand :: tl)
                    else (ts, tl)
                | Heap.Nil -> (ts, tl)

            let newTs, taskList = loop timerStore List.Empty
            timerStore <- newTs
            taskList
            |> List.map postCommand
            |> Async.Parallel
            |> Async.Ignore
            |> Async.StartImmediate

        do checker.Elapsed.Add handler
        do checker.AutoReset <- true
        do checker.Start()

        let validateTime time =
            if time < DateTime.Now
            then failwithf "The given time is in the past: %A" time

        let setTimer timerData =
            timerStore <- timerStore |> Heap.insert timerData

        let deleteTimer timerData =
            let rec loop ts ns =
                match ts with
                | Heap.Cons(h, t) ->
                    if h = timerData
                    then ns |> Heap.merge t
                    else loop t (ns |> Heap.insert h)
                | Heap.Nil -> Heap.empty minHeap
            timerStore <- loop timerStore (Heap.empty minHeap)

        interface ITimer with
            member _.Dispose() =
                checker.Dispose()

            member _.SetTimer timerData =
                validateTime timerData.Time
                async { setTimer timerData }

            member _.GetAllTimers = async {
                return timerStore |> Heap.toSeq
            }

            member _.DeleteTimer timerData = async {
                deleteTimer timerData
            }

            member _.UpdateTimer prev newData =
                validateTime newData.Time
                async {
                    deleteTimer prev
                    setTimer newData
                }

    let createInMemoryTimer millisec postCommand =
        new InMemoryTimer(millisec, postCommand) :> ITimer
