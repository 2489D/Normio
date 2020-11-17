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
        Time = DateTime.Now.AddSeconds(float 10)
        TaskCommand = StartExam(Guid.NewGuid())
    }

    let timerData2 = {
        Time = DateTime.Now.AddSeconds(float 20)
        TaskCommand = StartExam(Guid.NewGuid())
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

    let timerData = {
        Time = DateTime.Now.AddSeconds(float 3)
        TaskCommand = StartExam(Guid.NewGuid())
    }

    try
        inMemoryTimer.SetTimer timerData |> Async.RunSynchronously
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

    let timerData = {
        Time = past_3sec
        TaskCommand = StartExam(Guid.NewGuid())
    }

    fun () -> inMemoryTimer.SetTimer timerData |> Async.RunSynchronously
    |> should (throwWithMessage (sprintf "The given time is in the past: %A" past_3sec)) typeof<System.Exception>


[<Fact>]
let ``timer execute task 1`` () =
    let mutable result: Command list = List.Empty
    let postCommand command = async {
        result <- command :: result
    }
    use inMemoryTimer = createInMemoryTimer (float 100) postCommand

    let examId = Guid.NewGuid()
    let timerData = {
        Time = DateTime.Now.AddMilliseconds(float 300)
        TaskCommand = StartExam examId
    }

    inMemoryTimer.SetTimer timerData |> Async.RunSynchronously

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
    let timerData = {
        Time = DateTime.Now.AddMilliseconds(float 300)
        TaskCommand = StartExam examId
    }

    inMemoryTimer.SetTimer timerData |> Async.RunSynchronously |> ignore

    Threading.Thread.Sleep(100)
    result |> should be Empty
    Threading.Thread.Sleep(300)
    result |> should equal [StartExam examId]

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
    let timerData1 = {
        Time = DateTime.Now.AddMilliseconds(float 100)
        TaskCommand = StartExam examId1
    }

    let examId2 = Guid.NewGuid()
    let timerData2 = {
        Time = DateTime.Now.AddMilliseconds(float 200)
        TaskCommand = StartExam examId2
    }

    inMemoryTimer.SetTimer timerData1 |> Async.RunSynchronously
    inMemoryTimer.SetTimer timerData2 |> Async.RunSynchronously

    Threading.Thread.Sleep(interval + 100)
    result |> should equal [StartExam examId1] // some time Empty (need to sleep longer?)
    Threading.Thread.Sleep(100)
    result |> should equal [StartExam examId2; StartExam examId1]

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
let ``get all timers should return correctly`` () =
    let postCommand _ = () |> async.Return
    use inMemoryTimer = createInMemoryTimer (float 1000) postCommand

    let timerData1 = {
        Time = DateTime.Now.AddMilliseconds(float 100)
        TaskCommand = StartExam(Guid.NewGuid())
    }

    let timerData2 = {
        Time = DateTime.Now.AddMilliseconds(float 100)
        TaskCommand = StartExam(Guid.NewGuid())
    }

    inMemoryTimer.SetTimer timerData1 |> Async.RunSynchronously
    inMemoryTimer.SetTimer timerData2 |> Async.RunSynchronously

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

    let timerData = {
        Time = DateTime.Now.AddMilliseconds(float 100)
        TaskCommand = StartExam(Guid.NewGuid())
    }

    inMemoryTimer.SetTimer timerData |> Async.RunSynchronously

    inMemoryTimer.DeleteTimer timerData |> Async.RunSynchronously

    Threading.Thread.Sleep(2000);

[<Fact>]
let ``update timer should update timer`` () =
    let mutable result: Command list = List.Empty
    let postCommand command = async {
        result <- command :: result
    }
    use inMemoryTimer = createInMemoryTimer (float 100) postCommand

    let examId = Guid.NewGuid()
    let pastTimerData = {
        Time = DateTime.MaxValue
        TaskCommand = StartExam examId
    }

    let newTimerData = {
        Time = DateTime.Now.AddMilliseconds(float 100)
        TaskCommand = StartExam examId
    }

    inMemoryTimer.SetTimer pastTimerData |> Async.RunSynchronously

    inMemoryTimer.UpdateTimer pastTimerData newTimerData |> Async.RunSynchronously

    Threading.Thread.Sleep(200);
    result |> should equal [StartExam examId]
