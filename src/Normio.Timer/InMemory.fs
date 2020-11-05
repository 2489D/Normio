namespace Normio.Timer

open System
open FSharpx.Collections

[<AutoOpen>]
module InMemory =
    type private InMemoryTimer(secInterval) =
        let minHeap = false
        let mutable timerStore: Heap<TimerData> = Heap.empty minHeap

        let checker = new Timers.Timer(float (secInterval * 1000))

        let handler (e: Timers.ElapsedEventArgs) =
            let rec loop ts =
                match ts with
                | Heap.Cons(h, t) ->
                    if h.Time <= e.SignalTime then
                        h.Task |> Async.Start
                        loop t
                    else ts
                | Heap.Nil -> ts
            timerStore <- loop timerStore

        do checker.Elapsed.Add handler
        do checker.AutoReset <- true
        do checker.Start()

        interface ITimer with
            member _.SetTimer time task =
                if time < DateTime.Now
                then failwith "The given time is in the past"
                else
                    let id = Guid.NewGuid()
                    let td =
                        { Id = id
                          Time = time
                          Task = task }
                    timerStore <- timerStore |> Heap.insert td
                    id

            member _.TryGetTimer id =
                timerStore
                |> Heap.toSeq
                |> Seq.tryFind (fun td -> td.Id = id)
            
            member _.DeleteTimer id =
                failwith "Unimplemented"

            member _.GetAllTimers =
                timerStore
                |> Heap.toSeq
            
            member _.Dispose() =
                checker.Dispose()
                // printfn "dispose test"

    let createInMemoryTimer secInterval = (new InMemoryTimer(secInterval)) :> ITimer
