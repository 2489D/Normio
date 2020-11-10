module Normio.Tests.Timer_Tests

open System
open Xunit
open Xunit.Sdk
open FsUnit
open FsUnit.Xunit

open Normio.Timer

/// Timer Functionality
/// 1. Time, Task -> timer set up -> when the time arrives, execute the task
/// 2. Timer id -> timer delete -> deletes the timer
/// 3. Timer id, Time -> timer update -> updates time of the timer
/// 4. Timer Dispose -> terminate the timers

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
    result |> should equal 1
    Threading.Thread.Sleep(100)
    result |> should equal 101

// FIXME : concurrency problem
// do we need MailboxProcessor in timer?
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
