open System
open System.Threading

type TimerData = {
    Id: Guid
    Time: DateTime
    Task: Async<unit>
}

type ITimer =
    abstract Set: DateTime -> Async<unit> -> Guid

type InMemoryTimer() =
    let mutable timerStore: Map<Guid, TimerData> = Map.empty
    
    let checker = new System.Timers.Timer(float (1000 * 1))

    let handler _ =
        let now, later =
            timerStore
            |> Map.partition (fun _ td -> td.Time <= DateTime.Now)

        timerStore <- later

        now
        |> Map.toSeq
        |> Seq.map (fun (_, td) -> td.Task)
        |> Async.Parallel
        |> Async.RunSynchronously
        |> ignore

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
        let newStore = Map.add id td timerStore
        timerStore <- newStore
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

    Thread.Sleep 3000

    0
