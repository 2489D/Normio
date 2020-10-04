module Normio.Web.JsonFormatter

open System
open Normio.Core
open Normio.Core.Domain
open Normio.Core.States

open Suave
open Suave.Operators
open Suave.Successful
open Suave.RequestErrors
open Newtonsoft.Json.Linq

let (.=) key (value: obj) = JProperty(key, value)

let jobj jProperties =
    let jObject = JObject()
    jProperties |> List.iter jObject.Add
    jObject

let jArray jObjects =
    let jArray = JArray()
    jObjects |> List.iter jArray.Add
    jArray

let studentJObj (student: Student) =
    jobj [
        "id" .= student.Id
        "name" .= student.Name
    ]

let mapJArray toJObj (map: Map<Guid, _>) =
    map
    |> Map.toList
    |> List.map (fun (_, object) -> object |> toJObj)
    |> jArray

let hostJObj (host: Domain.Host) =
    jobj [
        "id" .= host.Id
        "name" .= host.Name
    ]

let examJObj exam =
    jobj [
        "title" .= exam.Title
        "examId" .= exam.Id
        "students" .= (exam.Students |> mapJArray studentJObj)
        "hosts" .= (exam.Hosts |> mapJArray hostJObj)
        "questions" .= (exam.Questions)
    ]

let examIdJObj (examId: Guid) =
    jobj [
        "examId" .= examId
    ]

let stateJObj = function
| ExamIsWaiting exam ->
    jobj [
        "state" .= "ExamIsWaiting"
        "data" .= (examJObj exam)
    ]
| ExamIsRunning exam ->
    jobj [
        "state" .= "ExamIsRunning"
        "data" .= (examJObj exam)
    ]
| ExamIsFinished exam ->
    jobj [
        "state" .= "ExamIsFinished"
        "data" .= (examJObj exam)
    ]
| ExamIsClose examId ->
    match examId with
    | Some id' ->
        jobj [
            "state" .= "ExamIsClosed"
            "data" .= id'
        ]
    | None ->
        jobj [
            "state" .= "ExamIsClosed"
        ]

let JSON webpart jsonString (context: HttpContext) = async {
    let wp =
        webpart jsonString
        >=> Writers.setMimeType "application/json; charset=utf-8"
    return! wp context
}

let toStateJson state =
    state
    |> stateJObj
    |> string
    |> JSON OK

let toErrorJson err =
    jobj [
        "error" .= err
    ]
    |> string |> JSON BAD_REQUEST