namespace Normio.Timer

open System
open System.Runtime.Serialization
open FSharpx.Collections

[<AutoOpen>]
[<KnownType("timer")>]
module InMemory =
    type private InMemoryTimer(millisec) =
        let minHeap = false
        let mutable timerStore: Heap<TimerData> = Heap.empty minHeap

        let checker = new Timers.Timer(millisec)

        let handler (e: Timers.ElapsedEventArgs) =
            let rec loop ts tl =
                match ts with
                | Heap.Cons(h, t) ->
                    if h.Time <= e.SignalTime then
                        loop t (h.Task :: tl)
                    else (ts, tl)
                | Heap.Nil -> (ts, tl)
            let newTs, taskList = loop timerStore List.Empty
            timerStore <- newTs
            taskList
            |> Async.Parallel
            |> Async.Ignore
            |> Async.StartImmediate

        do checker.Elapsed.Add handler
        do checker.AutoReset <- true
        do checker.Start()

        interface ITimer with
            member _.Dispose() =
                checker.Dispose()
                // printfn "dispose test"

            member _.SetTimer time task =
                if time < DateTime.Now
                then failwithf "The given time is in the past: %A" time
                else async {
                    let id = Guid.NewGuid()
                    let td =
                        { Id = id
                          Time = time
                          Task = task }
                    timerStore <- timerStore |> Heap.insert td
                    return id
                }

            member _.TryGetTimer id =
                timerStore
                |> Heap.toSeq
                |> Seq.tryFind (fun td -> td.Id = id)
                |> async.Return

            member _.GetAllTimers =
                timerStore
                |> Heap.toSeq
                |> async.Return

            // FIXME : is this the best way?
            member _.DeleteTimer id =
                let rec loop ts ns =
                    match ts with
                    | Heap.Cons(h, t) ->
                        if h.Id = id
                        then ns |> Heap.merge t
                        else loop t (ns |> Heap.insert h)
                    | Heap.Nil -> Heap.empty minHeap
                async {
                    timerStore <- loop timerStore (Heap.empty minHeap)
                }

            // FIXME : is this the best way?
            member _.UpdateTimer id time =
                let rec loop ts ns =
                    match ts with
                    | Heap.Cons(h, t) ->
                        if h.Id = id
                        then ns |> Heap.insert { h with Time = time } |> Heap.merge t
                        else loop t (ns |> Heap.insert h)
                    | Heap.Nil -> Heap.empty minHeap
                async {
                    timerStore <- loop timerStore (Heap.empty minHeap)
                }

    let createInMemoryTimer millisec = (new InMemoryTimer(millisec)) :> ITimer
