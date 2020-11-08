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
        inMemoryTimer.SetTimer (DateTime.Now.AddSeconds(float 3)) task |> ignore
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

    (fun () -> inMemoryTimer.SetTimer (DateTime.Now.AddSeconds(float -3)) task |> ignore)
    |> should (throwWithMessage "The given time is in the past") typeof<System.Exception>


[<Fact>]
let ``timer execute task 1`` () =
    use inMemoryTimer = createInMemoryTimer (float 1000)
    let mutable result = 0
    let task = async {
        result <- 1
    }

    inMemoryTimer.SetTimer (DateTime.Now.AddSeconds(float 3)) task |> ignore
    Threading.Thread.Sleep(4000)
    result |> should equal 1

[<Fact>]
let ``timer execute task 2`` () =
    use inMemoryTimer = createInMemoryTimer (float 1000)
    let mutable result = 0
    let task = async {
        result <- 1
    }

    inMemoryTimer.SetTimer (DateTime.Now.AddSeconds(float 3)) task |> ignore
    Threading.Thread.Sleep(1000)
    result |> should equal 0
    Threading.Thread.Sleep(3000)
    result |> should equal 1

[<Fact>]
let ``timer execute task 3`` () =
    let interval = 1000
    use inMemoryTimer = createInMemoryTimer (float interval)
    let mutable result = 0
    let task amount = async {
        result <- result + amount
    }

    inMemoryTimer.SetTimer (DateTime.Now.AddSeconds(float 1)) (task 1) |> ignore
    inMemoryTimer.SetTimer (DateTime.Now.AddSeconds(float 2)) (task 100) |> ignore
    Threading.Thread.Sleep(1 * 1000 + interval)
    result |> should equal 1
    Threading.Thread.Sleep(1 * 1000)
    result |> should equal 101
