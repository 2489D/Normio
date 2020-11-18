module Normio.Tests.Timer_Tests

open System
open Xunit
open FsUnit.Xunit

open Normio.Core.Commands
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
        Command = StartExam (Guid.NewGuid())
        Time = DateTime.Now.AddSeconds(float 10)
    }

    let timerData2 = {
        Command = StartExam (Guid.NewGuid())
        Time = DateTime.Now.AddSeconds(float 20)
    }

    timerData1 = timerData2 |> should equal false
    timerData1 <> timerData2 |> should equal true
    timerData1 < timerData2 |> should equal true
    timerData1 > timerData2 |> should equal false

[<Fact>]
let ``timer should be created`` () =
    let postCommand _ = async {
        failwith "this should not be executed"
    }

    // check tasks every 1 second
    use inMemoryTimer = createInMemoryTimer (float 1000) postCommand

    try
        inMemoryTimer.CreateTimer (StartExam (Guid.NewGuid())) (DateTime.Now.AddSeconds(float 3)) |> Async.RunSynchronously
    with
    | _ ->
        failwith "This should not fail"

[<Fact>]
let ``timer should not be created`` () =
    let postCommand _ = async {
        failwith "this should not be executed"
    }

    // check tasks every 1 second
    use inMemoryTimer = createInMemoryTimer (float 1000) postCommand

    let past_3sec = DateTime.Now.AddSeconds(float -3)

    fun () -> inMemoryTimer.CreateTimer (StartExam(Guid.NewGuid())) past_3sec |> Async.RunSynchronously
    |> should (throwWithMessage (sprintf "The given time is in the past: %A" past_3sec)) typeof<System.Exception>


[<Fact>]
let ``timer execute task 1`` () =
    let mutable result: Command list = List.Empty
    let postCommand command = async {
        result <- command :: result
    }

    use inMemoryTimer = createInMemoryTimer (float 100) postCommand

    let examId = Guid.NewGuid()

    inMemoryTimer.CreateTimer (StartExam examId) (DateTime.Now.AddMilliseconds(float 300)) |> Async.RunSynchronously

    Threading.Thread.Sleep(400)
    result |> should equal [StartExam examId]

[<Fact>]
let ``timer execute task 2`` () =
    let mutable result: Command list = List.Empty
    let postCommand command = async {
        result <- command :: result
    }

    use inMemoryTimer = createInMemoryTimer (float 100) postCommand
    
    let examId = Guid.NewGuid()

    inMemoryTimer.CreateTimer (StartExam examId) (DateTime.Now.AddMilliseconds(float 300)) |> Async.RunSynchronously |> ignore

    Threading.Thread.Sleep(100)
    result |> should be Empty
    Threading.Thread.Sleep(300)
    result |> should equal [StartExam examId] // some time Empty

// some time fails
[<Fact>]
let ``timer execute task 3`` () =
    let interval = 100
    let mutable result: Command list = List.Empty
    let postCommand command = async {
        result <- command :: result
    }

    use inMemoryTimer = createInMemoryTimer (float interval) postCommand

    let examId1 = Guid.NewGuid()
    let examId2 = Guid.NewGuid()
    let now = DateTime.Now
    inMemoryTimer.CreateTimer (StartExam examId1) (now.AddMilliseconds(float 100)) |> Async.RunSynchronously
    inMemoryTimer.CreateTimer (StartExam examId2) (now.AddMilliseconds(float 200)) |> Async.RunSynchronously

    Threading.Thread.Sleep(interval + 100)
    result |> should equal [StartExam examId1] // some time Empty (need to sleep longer?)
    Threading.Thread.Sleep(100)
    result |> should equal [StartExam examId2; StartExam examId1]

// FIXME : concurrency problem
// let ``timers should be set correctly even if more than one SetTimer called at the same time`` ()

[<Fact>]
let ``get all timers should return correctly`` () =
    let postCommand _ = () |> async.Return

    use inMemoryTimer = createInMemoryTimer (float 1000) postCommand

    let command1 = StartExam (Guid.NewGuid())
    let timerData1 = {
        Command = command1
        Time = DateTime.MaxValue
    }

    let command2 = StartExam (Guid.NewGuid())
    let timerData2 = {
        Command = command2
        Time = DateTime.MaxValue
    }

    inMemoryTimer.CreateTimer command1 DateTime.MaxValue |> Async.RunSynchronously
    inMemoryTimer.CreateTimer command2 DateTime.MaxValue |> Async.RunSynchronously

    let expectedSeq = seq {
        timerData1
        timerData2 }

    inMemoryTimer.GetAllTimers
    |> Async.RunSynchronously
    |> Seq.zip expectedSeq
    |> Seq.iter (fun (expected, actual) -> actual |> should equal expected)

[<Fact>]
let ``delete timer should delete timer`` () =
    let postCommand _ = async {
        failwith "this should not be executed"
    }

    use inMemoryTimer = createInMemoryTimer (float 100) postCommand

    inMemoryTimer.CreateTimer (StartExam (Guid.NewGuid())) (DateTime.Now.AddMilliseconds(float 100)) |> Async.RunSynchronously

    inMemoryTimer.DeleteTimer (StartExam (Guid.NewGuid())) |> Async.RunSynchronously

    Threading.Thread.Sleep(200);

[<Fact>]
let ``update timer should update timer`` () =
    let mutable result: Command list = List.Empty
    let postCommand command = async {
        result <- command :: result
    }

    use inMemoryTimer = createInMemoryTimer (float 100) postCommand

    let examId = Guid.NewGuid()

    inMemoryTimer.CreateTimer (StartExam examId) DateTime.MaxValue |> Async.RunSynchronously

    inMemoryTimer.UpdateTimer (StartExam examId) (DateTime.Now.AddMilliseconds(float 100)) |> Async.RunSynchronously

    Threading.Thread.Sleep(200);
    result |> should equal [StartExam examId]
