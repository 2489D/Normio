module Normio.Tests.Timer_Tests

open System
open Xunit
open Xunit.Sdk
open FsUnit
open FsUnit.Xunit

open Normio.Timer

/// Timer Functionality
/// 1. Time, Task -> timer set up -> when the time arrives, execute the task

[<Fact>]
let ``timer be created`` () =
    // check tasks every 1 second
    use inMemoryTimer = createInMemoryTimer 1
    let task = async {
        do printfn "this should be executed"
    }
    
    try
        inMemoryTimer.SetTimer (DateTime.Now.AddSeconds(float 3)) task |> ignore
    with
    | _ ->
        failwith "This should not fail"

[<Fact>]
let ``timer not be created`` () =
    // check tasks every 1 second
    use inMemoryTimer = createInMemoryTimer 1
    let task = async {
        do printfn "this should be executed"
    }
    
    try
        inMemoryTimer.SetTimer (DateTime.Now.AddSeconds(float -3)) task |> ignore
        failwith "This should not pass"
    with
    | _ -> ()

[<Fact>]
let ``timer execute task 1`` () =
    use inMemoryTimer = createInMemoryTimer 1
    let mutable result = 0
    let task = async {
        result <- 1
    }
    
    inMemoryTimer.SetTimer (DateTime.Now.AddSeconds(float 3)) task |> ignore
    Threading.Thread.Sleep(4000)
    Assert.Equal(result, 1)
    
    
[<Fact>]
let ``timer execute task 2`` () =
    use inMemoryTimer = createInMemoryTimer 1
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
    // 0
    let interval = 1
    use inMemoryTimer = createInMemoryTimer interval
    let mutable result = 0
    let task amount = async {
        result <- result + amount
    }
    
    inMemoryTimer.SetTimer (DateTime.Now.AddSeconds(float 1)) (task 1) |> ignore
    inMemoryTimer.SetTimer (DateTime.Now.AddSeconds(float 2)) (task 100) |> ignore
    Threading.Thread.Sleep((1 + interval) * 1000)
    Assert.Equal(1, result)
    Threading.Thread.Sleep(1 * 1000)
    Assert.Equal(101, result)

