module Normio.Web.Dev.JsonFormatter

open Normio.Core.Domain
open Normio.Core.Events
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

let studentJObj (host: Student) =
    jobj [
        "id" .= host.Id
        "name" .= host.Name
    ]

let hostJObj (host: Host) =
    jobj [
        "id" .= host.Id
        "name" .= host.Name
    ]

let eventJson = function
| ExamOpened (examId, title) ->
    jobj [
        "event" .= "examOpened"
        "examId" .= examId
        "title" .= title
    ]
| ExamStarted examId ->
    jobj [
        "event" .= "examStarted"
        "examId" .= examId
    ]
| ExamEnded examId ->
    jobj [
        "event" .= "examEnded"
        "examId" .= examId
    ]
| ExamClosed examId ->
    jobj [
        "event" .= "examClosed"
        "examId" .= examId
    ]
| StudentEntered (examId, student) ->
    jobj [
        "event" .= "studentEntered"
        "examId" .= examId
        "student" .= (student |> studentJObj)
    ]
| StudentLeft (examId, studentId) ->
    jobj [
        "event" .= "studentLeft"
        "examId" .= examId
        "studentId" .= studentId
    ]
| HostEntered (examId, host) ->
    jobj [
        "event" .= "hostEntered"
        "examId" .= examId
        "host" .= (host |> hostJObj)
    ]
| HostLeft (examId, hostId) ->
    jobj [
        "event" .= "studentLeft"
        "examId" .= examId
        "hostId" .= hostId
    ]
| QuestionCreated (examId, file) ->
    jobj [
        "event" .= "questionCreated"
        "examId" .= examId
        "file" .= file
    ]
| QuestionDeleted (examId, file) ->
    jobj [
        "event" .= "questionDeleted"
        "examId" .= examId
        "file" .= file
    ]
| TitleChanged (examId, title) ->
    jobj [
        "event" .= "titleChanged"
        "examId" .= examId
        "title" .= title
    ]

