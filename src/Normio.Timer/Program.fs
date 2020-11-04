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

type ITimer =
    abstract Set: DateTime -> Async<unit> -> Guid

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

    member this.Set time task =
        let id = Guid.NewGuid()
        let td = {
            Id = id
            Time = time
            Task = task
        }
        timerStore <- timerStore |> Heap.insert td
        id

let timerObj = InMemoryTimer()

let inMemoryTimer =
    { new ITimer with
        member this.Set time task = timerObj.Set time task}

[<EntryPoint>]
let main argv =
    let asyncPrint s = async {
        printfn s
    }

    let testId1 =
        asyncPrint "timer 1"
        |> inMemoryTimer.Set (DateTime.Now.AddSeconds (float 1))

    let testId2 =
        asyncPrint "timer 2"
        |> inMemoryTimer.Set (DateTime.Now.AddSeconds (float 2))

    Threading.Thread.Sleep 3000

    0
