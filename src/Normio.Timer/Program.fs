module Normio.Timer.App

open System
open FSharpx.Collections

[<CustomEquality; CustomComparison>]
type TimerData = {
    Id: Guid
    Time: DateTime
    Task: Async<unit>
} with
    override this.Equals(other) =
        match other with
        | :? TimerData as td -> this.Id = td.Id
        | _ -> false
    override this.GetHashCode() = hash this.Id
    interface IComparable with
        override this.CompareTo(other) =
            DateTime.Compare(this.Time, (other :?> TimerData).Time)

// TODO : has to select what are really needed
// TODO : error handling
type ITimer =
    abstract SetTimer: DateTime -> Async<unit> -> Guid
    abstract GetTimer: Guid -> TimerData option
    abstract GetAllTimers: seq<TimerData>
    // TODO : how to update element in heap?
    // abstract UpdateTime: Guid -> DateTime -> unit
    // abstract UpdateTask: Guid -> Async<unit> -> unit

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
        let id = Guid.NewGuid()
        let td = {
            Id = id
            Time = time
            Task = task
        }
        timerStore <- timerStore |> Heap.insert td
        id

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

[<EntryPoint>]
let main argv =
    let asyncPrint s = async {
        printfn s
    }

    let testId1 =
        asyncPrint "timer 1"
        |> inMemoryTimer.SetTimer (DateTime.Now.AddSeconds (float 1))

    let testId2 =
        asyncPrint "timer 2"
        |> inMemoryTimer.SetTimer (DateTime.Now.AddSeconds (float 2))

    (*
    inMemoryTimer.GetAllTimers
    |> Seq.iter (fun td -> printfn "%s" (td.Id.ToString()))
    *)

    Threading.Thread.Sleep 3000

    0
