open System.Threading

let startTimer interval task =
    [Async.Sleep interval; task]
    |> Async.Sequential
    |> Async.Ignore
    |> Async.Start

[<EntryPoint>]
let main argv =
    let asyncPrint s = async {
        printfn s
    }

    asyncPrint "timer 1"
    |> startTimer 1000

    asyncPrint "timer 2"
    |> startTimer 2000

    Thread.Sleep 3000

    0
