namespace Normio.Timer

open System

[<AutoOpen>]
module App =
    let asyncPrint s = async {
        printfn s
    }

    [<EntryPoint>]
    let main argv =
        use inMemoryTimer = createInMemoryTimer 1

        let timerId1 =
            asyncPrint "timer 1"
            |> inMemoryTimer.SetTimer (DateTime.Now.AddSeconds (float 1))

        let timerId2 =
            asyncPrint "timer 2"
            |> inMemoryTimer.SetTimer (DateTime.Now.AddSeconds (float 2))

        (*
        inMemoryTimer.GetAllTimers
        |> Seq.iter (fun td -> printfn "%s" (td.Id.ToString()))
        *)

        Threading.Thread.Sleep 3000

        0
