module Normio.Timer.InMemory

open System
open FSharpx.Collections
open Normio.Timer.Domain
open Normio.Timer.Timer
open Normio.Timer.Errors

type InMemoryTimer() =
    let mutable timerStore: Heap<TimerData> = Heap.empty false
    
    let checker = new Timers.Timer(float (1000 * 1))

    let handler _ =
        let rec loop ts =
            match ts with
            | Heap.Cons(h, t) ->
                if h.Time <= DateTime.Now then
                    h.Task |> Async.Start
                    loop t
                else ts
            | Heap.Nil -> ts
        timerStore <- timerStore |> loop

    do checker.Elapsed.Add handler
    do checker.AutoReset <- true
    do checker.Start()

    member this.SetTimer time task =
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

    member this.GetTimer id =
        timerStore
        |> Heap.toSeq
        |> Seq.tryFind (fun td -> td.Id = id)

    member this.GetAllTimers =
        timerStore
        |> Heap.toSeq

let timerObj = InMemoryTimer()

let inMemoryTimer =
    { new ITimer with
        member this.SetTimer time task = timerObj.SetTimer time task
        member this.GetTimer id = timerObj.GetTimer id
        member this.GetAllTimers = timerObj.GetAllTimers}
