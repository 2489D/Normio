module Normio.Tests.Core

open System
open FsUnit
open Xunit
open Xunit.Sdk

open Normio.Core
open Normio.Core.States
open Normio.Core.Commands
open Normio.Tests.DSL
open Normio.Tests.Mocks

let newGuid () = Guid.NewGuid ()

type ResultBuilder() =
    member this.Bind (m, f) = Result.bind f m
    member this.Zero: Result<unit, unit> = Ok ()
    member this.Return x = Ok x
    member this.ReturnFrom x = x

let result = ResultBuilder()

[<Fact>]
let ``ExamIsClose -> OpenExam -> ExamIsWaiting`` () =
    given (ExamIsClose None)
    |> applied (OpenExam(newGuid(), validTitle))
    |> thenShouldBeOk

let createWaitingExam () =
    let testId = newGuid ()
    let exam =
        Exam.Initial testId validTitle
        |> ExamIsWaiting
    (exam, testId)
    
[<Fact>]
let ``ExamIsWaiting without a question can not start`` () =
    let testId = newGuid ()
    let waitingExam =
        ExamIsWaiting (Exam.Initial testId validTitle)
    given waitingExam
    |> applied (StartExam testId)
    |> thenShouldBeError

[<Fact>]
let ``ExamIsWaiting without a student can not start`` () =
    let testId = newGuid ()
    let waitingExam =
        ExamIsWaiting (Exam.Initial testId validTitle)
    given waitingExam
    |> applied (StartExam testId)
    |> thenShouldBeError

[<Fact>]
let ``ExamIsWaiting without a host can not start`` () =
    let testId = newGuid ()
    let waitingExam =
        ExamIsWaiting (Exam.Initial testId validTitle)
    given waitingExam
    |> applied (StartExam testId)
    |> thenShouldBeError

[<Fact>]
let ``Empty string should not be a valid exam title`` () =
    match "" |> ExamTitle40.create with
    | Ok _ ->
        XunitException("Empty string can not be a title")
        |> raise
    | Error _ -> ()
    
[<Fact>]
let ``Too long string should not be a valid exam title`` () =
    let tooLong = String.init 41 (fun _ -> "1")
    tooLong |> ExamTitle40.create
    |> thenShouldBeError

[<Fact>]
let ``Empty string should not be a valid user name`` () =
    "" |> UserName40.create
    |> thenShouldBeError

[<Fact>]
let ``Too long string should not be a valid user name`` () =
    let tooLong = String.init 41 (fun _ -> "1")
    tooLong |> UserName40.create
    |> thenShouldBeError
