module Normio.Core.Tests

open NUnit.Framework
open FsUnit
open Normio.Domain
open Normio.States
open Normio.Errors
open Normio.Events
open Normio.Commands
open Normio.CommandHandlers
open System
open Normio.Core.TestHelpers

let guid = Guid.NewGuid()

[<SetUp>]
let Setup () =
    ()

[<Test>]
let ``Can StartExam produce (RoomOnExam, [ExamStarted])`` () =
    Given (RoomIsWaiting Room.init)
    |> When (StartExam guid)
    |> ThenStateShouldBe (RoomOnExam Room.init)
    |> WithEvents [ExamStarted]

[<Test>]
let ``Does StartExam fail if the exam is already running`` () =
    Given (RoomOnExam Room.init)
    |> When (StartExam guid)
    |> ShouldFailWith ExamAlreadyStarted

[<Test>]
let ``Does EndExam fail if the exam is not running`` () =
    Given (RoomIsWaiting Room.init)
    |> When (EndExam guid)
    |> ShouldFailWith CannotEndExam 

[<Test>]
let ``Can EndExam produce (RoomExamFinished, [ExamEnded])`` () =
    Given (RoomOnExam Room.init)
    |> When (EndExam guid)
    |> ThenStateShouldBe (RoomExamFinished Room.init)
    |> WithEvents [ExamEnded]

