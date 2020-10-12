module Normio.Web.Dev.JsonFormatter

open System

open Normio.Core.Domain
open Normio.Core.Events
open Normio.Core.States
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
let fileJObj (file: File) =
    jobj [
        // TODO
    ]

let studentsJObj (students: Map<Guid, Student>) =
    let studentsList = students
                    |> Map.toList
                    |> List.map (function (_, s) -> s)
                    |> List.map studentJObj
    jArray studentsList

let hostsJObj (hosts: Map<Guid, Host>) =
    let hostsList = hosts
                    |> Map.toList
                    |> List.map (function (_, s) -> s)
                    |> List.map hostJObj
    jArray hostsList
    
let questionsJObj (questions: File List) =
    jArray (questions |> List.map fileJObj)
    

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


let stateJson = function
| ExamIsClose (examId) ->
    jobj[
        "state" .= "examIsClosed"
        "examId" .= (match examId with | Some t -> sprintf "%A" t | None -> "") // ???
    ]
| ExamIsWaiting (exam) ->
    jobj[
        "state" .= "examIsWaiting"
        "examId" .= exam.Id
        "examTitle" .= exam.Title
        "questions" .= (exam.Questions |> questionsJObj)
        "students" .= (exam.Students |> studentsJObj)
        "hosts" .= (exam.Hosts |> hostsJObj)
    ]
| ExamIsRunning (exam) ->
    jobj[
        "state" .= "examIsRunning"
        "examId" .= exam.Id
        "examTitle" .= exam.Title
        "questions" .= (exam.Questions |> questionsJObj)
        "students" .= (exam.Students |> studentsJObj)
        "hosts" .= (exam.Hosts |> hostsJObj)
    ]
| ExamIsFinished (exam) ->
    jobj[
        "state" .= "examIsFinished"
        "examId" .= exam.Id
        "examTitle" .= exam.Title
        "questions" .= (exam.Questions |> questionsJObj)
        "students" .= (exam.Students |> studentsJObj)
        "hosts" .= (exam.Hosts |> hostsJObj)
    ]
