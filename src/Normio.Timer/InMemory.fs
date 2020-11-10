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
            // FIXME : tail rec
            member _.DeleteTimer id = async {
                let rec loop ts =
                    match ts with
                    | Heap.Cons(h, t) ->
                        if h.Id = id then t
                        else (loop t).Insert h
                    | Heap.Nil -> Heap.empty minHeap
                timerStore <- loop timerStore
            }

            // FIXME : is this the best way?
            // FIXME : tail rec
            member _.UpdateTimer id time = async {
                let rec loop ts =
                    match ts with
                    | Heap.Cons(h, t) ->
                        if h.Id = id
                        then t.Insert { h with Time = time }
                        else (loop t).Insert h
                    | Heap.Nil -> Heap.empty minHeap
                timerStore <- loop timerStore
            }

    let createInMemoryTimer secInterval = (new InMemoryTimer(secInterval)) :> ITimer
