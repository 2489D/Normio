open System
open Normio.Storage
open Suave
open Suave.Json
open Suave.Filters
open Suave.Operators
open Suave.Successful

open Normio.Web.Requests

open Normio.States
let tryGetExamFromState = function
    | ExamIsWaiting exam -> Some exam
    | ExamIsRunning exam -> Some exam
    | ExamIsFinished exam -> Some exam
    | ExamIsClose id -> None

let getStudents (store: EventStore) =
    let inner (store: EventStore) (req: GetStudents) =
        match EventStore.tryGetState store req.ExamId with
        | Ok state ->
            match tryGetExamFromState state with
            | Some exam -> Ok exam.Students
            | None -> Error "Exam is closed"
        | Error msg -> Error msg
    mapJson (inner store)

let createStudent (context: HttpContext) = async.Return
let deleteStudent (context: HttpContext) = async.Return

let app =
    let store = EventStore.createStorage()
    path "/exams" >=> choose [
        path "/students" >=> choose [
            GET >=> choose [
                path "/" >=> (getStudents store)
            ]
            POST >=> createStudent
            DELETE >=> deleteStudent
        ]
        path "/hosts" >=> choose [
            GET >=> getHost
            POST >=> createHost
            DELETE >=> deleteHost
        ]
        path "/questions" >=> choose [
            GET >=> getQuestions
            POST 
        ]
    ]


[<EntryPoint>]
let main argv =
    startWebServer defaultConfig app
    0 // return an integer exit code
