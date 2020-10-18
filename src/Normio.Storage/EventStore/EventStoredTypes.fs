module Normio.Storage.EventStore.EventStoredTypes

open System
open System.Text.Json.Serialization
open FSharp.Data
open Normio.Core.Domain
open Normio.Core.Events

[<CLIMutable>]
type EventStored = {
    [<JsonPropertyName("id")>]
    Id : Guid // unique identifier for an event data
    [<JsonPropertyName("event")>]
    Event : string // a name of an event
    [<JsonPropertyName("examId")>]
    ExamId: Guid // the id of exam which an event references to
    [<JsonPropertyName("data")>]
    Data : string
}

[<Literal>]
let ExamOpenedDataJson = """ {
    "title" : "exam title example"
} """
type ExamOpenedData = JsonProvider<ExamOpenedDataJson>

[<Literal>]
let StudentEnteredDataJson = """ {
    "studentId" : "c752f0f5-25dc-4c7e-b392-470d36701efc",
    "name": "John"
} """
type StudentEnteredData = JsonProvider<StudentEnteredDataJson>

[<Literal>]
let StudentLeftDataJson = """ {
    "studentId" : "c752f0f5-25dc-4c7e-b392-470d36701efc"
} """
type StudentLeftData = JsonProvider<StudentLeftDataJson>

[<Literal>]
let HostEnteredDataJson = """ {
    "hostId" : "c752f0f5-25dc-4c7e-b392-470d36701efc",
    "name" : "Bob"
} """
type HostEnteredData = JsonProvider<HostEnteredDataJson>

[<Literal>]
let HostLeftDataJson = """ {
    "hostId" : "c752f0f5-25dc-4c7e-b392-470d36701efc"
} """
type HostLeftData = JsonProvider<HostLeftDataJson>

[<Literal>]
let SubmissionCreatedDataJson = """ {
    "submissionId" : "c752f0f5-25dc-4c7e-b392-470d36701efc",
    "examId" : "c752f0f5-25dc-4c7e-b392-470d36701efc",
    "student" : {
        "studentId" : "c752f0f5-25dc-4c7e-b392-470d36701efc",
        "name" : "John"
    },
    "file" : {
        "fileId" : " c752f0f5-25dc-4c7e-b392-470d36701efc",
        "name" : "script.fsx"
    }
} """
type SubmissionCreatedData = JsonProvider<SubmissionCreatedDataJson>

[<Literal>]
let QuestionCreatedDataJson = """ {
    "fileId" : "c752f0f5-25dc-4c7e-b392-470d36701efc",
    "name" : "question.txt"
} """
type QuestionCreatedData = JsonProvider<QuestionCreatedDataJson>

[<Literal>]
let QuestionDeletedDataJson = """ {
    "fileId" : "c752f0f5-25dc-4c7e-b392-470d36701efc"
} """
type QuestionDeletedData = JsonProvider<QuestionDeletedDataJson>

[<Literal>]
let TitleChangedDataJson = """ {
    "title" : "new title example"
} """
type TitleChangedJson = JsonProvider<TitleChangedDataJson>

let (|ExamOpenedStored|_|) eventStored: Event option =
    match eventStored.Event with
    | "ExamOpened" ->
        let data = ExamOpenedData.Parse(eventStored.Data).Title
        match ExamTitle40.create data with
        | Ok title ->
            ExamOpened (eventStored.ExamId, title) |> Some
        | Error err -> None
    | _ -> None

let (|ExamStartedStored|_|) eventStored: Event option =
    match eventStored.Event with
    | "ExamStarted" ->
        ExamStarted eventStored.ExamId |> Some
    | _ -> None

let (|ExamEndedStored|_|) eventStored =
    match eventStored.Event with
    | "ExamEnded" ->
        ExamEnded eventStored.ExamId |> Some
    | _ -> None

let (|ExamClosedStored|_|) eventStored =
    match eventStored.Event with
    | "ExamClosed" ->
        ExamClosed eventStored.ExamId |> Some
    | _ -> None

let (|StudentEnteredStored|_|) eventStored =
    match eventStored.Event with
    | "StudentEntered" ->
        let data = StudentEnteredData.Parse(eventStored.Data)
        match UserName40.create data.Name with
        | Ok name ->
            let student: Student =
                { Id = data.StudentId
                  Name = name }
            StudentEntered (eventStored.ExamId, student) |> Some
        | _ -> None
    | _ -> None

let (|StudentLeftStored|_|) eventStored =
    match eventStored.Event with
    | "StudentLeft" ->
        let data = StudentLeftData.Parse(eventStored.Data)
        StudentLeft (eventStored.ExamId, data.StudentId) |> Some
    | _ -> None

let (|HostEnteredStored|_|) eventStored =
    match eventStored.Event with
    | "HostEntered" ->
        let data = HostEnteredData.Parse(eventStored.Data)
        match UserName40.create data.Name with
        | Ok name -> 
            let host: Host =
                { Id = data.HostId
                  Name = name }
            HostEntered (eventStored.ExamId, host) |> Some
        | _ -> None
    | _ -> None

let (|HostLeftStored|_|) eventStored =
    match eventStored.Event with
    | "HostLeft" ->
        let data = HostLeftData.Parse(eventStored.Data)
        HostLeft (eventStored.ExamId, data.HostId) |> Some
    | _ -> None

let (|SubmissionCreatedStored|_|) eventStored =
    match eventStored.Event with
    | "SubmissionCreated" ->
        let data = SubmissionCreatedData.Parse(eventStored.Data)
        match UserName40.create data.Student.Name with
        | Ok studentName ->
            let student : Student =
                { Id = data.Student.StudentId
                  Name = studentName }
            match FileString200.create data.File.Name with
            | Ok fileName ->
                let file =
                    { Id = data.File.FileId
                      FileName = fileName }
                let submission =
                    { Id = data.SubmissionId
                      ExamId = data.ExamId
                      Student = student
                      File = file }
                SubmissionCreated (eventStored.ExamId, submission) |> Some
            | _ -> None
        | _ -> None
    | _ -> None

let (|QuestionCreatedStored|_|) eventStored =
    match eventStored.Event with
    | "QuestionCreated" ->
        let data = QuestionCreatedData.Parse(eventStored.Data)
        match FileString200.create data.Name with
        | Ok fileName ->
            let file =
                { Id = data.FileId
                  FileName = fileName }
            QuestionCreated (eventStored.ExamId, file) |> Some
        | _ -> None
    | _ -> None
        
let (|QuestionDeletedStored|_|) eventStored =
    match eventStored.Event with
    | "QuestionDeleted" ->
        let data = QuestionDeletedData.Parse(eventStored.Data)
        QuestionDeleted (eventStored.ExamId, data.FileId) |> Some
    | _ -> None

let (|TitleChangedStored|_|) eventStored =
    match eventStored.Event with
    | "TitleChanged" ->
        let data = TitleChangedJson.Parse(eventStored.Data)
        match ExamTitle40.create data.Title with
        | Ok title ->
            TitleChanged (eventStored.ExamId, title) |> Some
        | _ -> None
    | _ -> None
        
