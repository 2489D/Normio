module Normio.Tests.E2E

open System
open System.Threading.Tasks
open FSharp.Data
open FSharp.Data.JsonExtensions
open Xunit
open FsUnit
open FsUnit.Xunit
open FsHttp

let testUrl = "http://localhost:6546/"

let getString (response: Response) =
    response.content.ReadAsStringAsync()
    |> Async.AwaitTask
    |> Async.RunSynchronously

let openExamResponse () =
    http {
        POST (testUrl + "api/openExam")
        body
        json """ { "title" : "sample" } """
    }
    |> getString

let startExamResponse examId =
    let startExamRequestBody = sprintf """ { "examId" : %s }""" examId
    http {
        POST (testUrl + "api/startExam")
        body
        json startExamRequestBody
    }
    |> getString

[<Fact>]
let ``simple 1`` () =
    let json = openExamResponse() |> JsonValue.Parse
    let examId = json?ExamIsWaiting?id |> string
    printfn "%A" examId
    startExamResponse examId
    |> printfn "%s"

[<Fact>]
let ``simple 2`` () =
    let json = openExamResponse() |> JsonValue.Parse
    let examId = json?ExamIsWaiting?id |> string
    printfn "%A" examId
    startExamResponse examId
    |> printfn "%s"
