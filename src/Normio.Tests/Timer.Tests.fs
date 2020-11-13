module Normio.Tests.Timer_Tests

open System
open Xunit
open FsUnit.Xunit

open Normio.Timer

/// Timer Functionality
/// 1. millisec interval -> createInMemoryTimer -> ITimer object
/// 2. time, task -> set timer -> when the time arrives, execute the task
/// 3. timer id -> try get timer -> timer data option
/// 4. get all timers -> timer data sequence
/// 5. timer id -> delete timer -> deletes the timer
/// 6. timer id, time -> update timer -> updates time of the timer
/// 7. timer dispose -> terminate the timers

[<Fact>]
let ``timer data should be compared correctly`` () =
    let timerData1 = {
        Id = Guid.NewGuid()
        Time = DateTime.Now.AddSeconds(float 10)
        Task = () |> async.Return
    }

    let timerData2 = {
        Id = Guid.NewGuid()
        Time = DateTime.Now.AddSeconds(float 20)
        Task = () |> async.Return
    }

    timerData1 = timerData2 |> should equal false
    timerData1 <> timerData2 |> should equal true
    timerData1 < timerData2 |> should equal true
    timerData1 > timerData2 |> should equal false

[<Fact>]
let ``timer should be created`` () =
    // check tasks every 1 second
    use inMemoryTimer = createInMemoryTimer (float 1000)
    let task = async {
        failwith "this should not be executed"
    }

    try
        inMemoryTimer.SetTimer (DateTime.Now.AddSeconds(float 3)) task |> Async.RunSynchronously |> ignore
    with
    | _ ->
        failwith "This should not fail"

[<Fact>]
let ``timer should not be created`` () =
    // check tasks every 1 second
    use inMemoryTimer = createInMemoryTimer (float 1000)
    let task = async {
        failwith "this should not be executed"
    }

    let past_3sec = DateTime.Now.AddSeconds(float -3)

    (fun () -> inMemoryTimer.SetTimer past_3sec task |> Async.RunSynchronously |> ignore)
    |> should (throwWithMessage (sprintf "The given time is in the past: %A" past_3sec)) typeof<System.Exception>


[<Fact>]
let ``timer execute task 1`` () =
    use inMemoryTimer = createInMemoryTimer (float 100)
    let mutable result = 0
    let task = async {
        result <- 1
    }

    inMemoryTimer.SetTimer (DateTime.Now.AddMilliseconds(float 300)) task |> Async.RunSynchronously |> ignore

    Threading.Thread.Sleep(400)
    result |> should equal 1

[<Fact>]
let ``timer execute task 2`` () =
    use inMemoryTimer = createInMemoryTimer (float 100)
    let mutable result = 0
    let task = async {
        result <- 1
    }

    inMemoryTimer.SetTimer (DateTime.Now.AddMilliseconds(float 300)) task |> Async.RunSynchronously |> ignore

    Threading.Thread.Sleep(100)
    result |> should equal 0
    Threading.Thread.Sleep(300)
    result |> should equal 1

// some time fails
[<Fact>]
let ``timer execute task 3`` () =
    let interval = 100
    use inMemoryTimer = createInMemoryTimer (float interval)
    let mutable result = 0
    let task amount = async {
        result <- result + amount
    }

    inMemoryTimer.SetTimer (DateTime.Now.AddMilliseconds(float 100)) (task 1) |> Async.RunSynchronously |> ignore
    inMemoryTimer.SetTimer (DateTime.Now.AddMilliseconds(float 200)) (task 100) |> Async.RunSynchronously |> ignore

    Threading.Thread.Sleep(interval + 100)
    result |> should equal 1    // some time 0 (need to sleep longer?)
    Threading.Thread.Sleep(100)
    result |> should equal 101

// FIXME : concurrency problem
// do we need MailboxProcessor in timer?
(*
[<Fact>]
let ``timers should be set correctly even if more than one SetTimer called at the same time`` () =
    let interval = 100
    use inMemoryTimer = createInMemoryTimer (float interval)
    let mutable result = 0
    let task amount = async {
        result <- result + amount
    }

    [inMemoryTimer.SetTimer (DateTime.Now.AddMilliseconds(float 100)) (task 1);
    inMemoryTimer.SetTimer (DateTime.Now.AddMilliseconds(float 200)) (task 100)]
    |> Async.Parallel
    |> Async.RunSynchronously
    |> ignore

    Threading.Thread.Sleep(interval + 100)
    result |> should equal 1    // FIXME : usually 100, sometimes 101 (why?) FUCK YOU
    Threading.Thread.Sleep(100)
    result |> should equal 101
*)

[<Fact>]
let ``try get timer should get timer data`` () =
    use inMemoryTimer = createInMemoryTimer (float 1000)

    let time = DateTime.Now.AddSeconds(float 10)
    let task = () |> async.Return
    let timerId =
        inMemoryTimer.SetTimer time task
        |> Async.RunSynchronously

    inMemoryTimer.TryGetTimer timerId
    |> Async.RunSynchronously
    |> should equal ({Id = timerId; Time = time; Task = task} |> Some)

[<Fact>]
let ``try get timer should return None`` () =
    use inMemoryTimer = createInMemoryTimer (float 1000)

    inMemoryTimer.TryGetTimer (Guid.NewGuid())
    |> Async.RunSynchronously
    |> should equal None

[<Fact>]
let ``get all timers should return correctly`` () =
    use inMemoryTimer = createInMemoryTimer (float 1000)

    let time1 = DateTime.Now.AddSeconds(float 10)
    let task1 = () |> async.Return
    let id1 = inMemoryTimer.SetTimer time1 task1 |> Async.RunSynchronously

    let time2 = DateTime.Now.AddSeconds(float 20)
    let task2 = () |> async.Return
    let id2 = inMemoryTimer.SetTimer time2 task2 |> Async.RunSynchronously

    let expectedSeq =
        [{Id = id1; Time = time1; Task = task1};
        {Id = id2; Time = time2; Task = task2}]
        |> Seq.ofList

    inMemoryTimer.GetAllTimers
    |> Async.RunSynchronously
    |> Seq.zip expectedSeq
    |> Seq.iter (fun (expected, actual) -> actual |> should equal expected)

[<Fact>]
let ``delete timer should delete timer`` () =
    let task = async {
        failwith "this should not be executed"
    }
    use inMemoryTimer = createInMemoryTimer (float 100)

    let timerId =
        inMemoryTimer.SetTimer (DateTime.Now.AddSeconds(float 1)) task
        |> Async.RunSynchronously

    inMemoryTimer.DeleteTimer timerId
    |> Async.RunSynchronously

    Threading.Thread.Sleep(2000);

[<Fact>]
let ``update timer should update time`` () =
    use inMemoryTimer = createInMemoryTimer (float 100)
    let mutable result = 0
    let task = async {
        result <- 1
    }

    let timerId =
        inMemoryTimer.SetTimer (DateTime.MaxValue) task
        |> Async.RunSynchronously

    inMemoryTimer.UpdateTimer timerId (DateTime.Now.AddSeconds(float 1))
    |> Async.RunSynchronously

    Threading.Thread.Sleep(2000);
    result |> should equal 1
