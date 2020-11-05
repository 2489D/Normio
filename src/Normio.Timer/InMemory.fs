namespace Normio.Timer.InMemory

open System
open FSharpx.Collections
open Normio.Timer.Domain
open Normio.Timer.Timer
open Normio.Timer.Errors

[<AutoOpen>]
module InMemory =
    type private InMemoryTimer(secInterval) =
        let mutable timerStore: Heap<TimerData> = Heap.empty false

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
            timerStore <- timerStore |> loop

        do checker.Elapsed.Add handler
        do checker.AutoReset <- true
        do checker.Start()

        interface ITimer with
            member _.SetTimer time task =
                if time < DateTime.Now
                then CannotSetTimer "Given time is Past" |> Error
                else
                    let id = Guid.NewGuid()
                    let td = {
                        Id = id
                        Time = time
                        Task = task
                    }
                    timerStore <- timerStore |> Heap.insert td
                    id |> Ok

            member _.GetTimer id =
                timerStore
                |> Heap.toSeq
                |> Seq.tryFind (fun td -> td.Id = id)

            member _.GetAllTimers =
                timerStore
                |> Heap.toSeq

            member _.Dispose() =
                checker.Dispose()
                // printfn "dispose test"

    let createInMemoryTimer secInterval = (new InMemoryTimer(secInterval)) :> ITimer
