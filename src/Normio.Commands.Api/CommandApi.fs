module Normio.Commands.Api.CommandApi

open System
open System.Text
open Normio.Core.States
open Normio.Core.Events
open Normio.Commands.Api.CommandHandlers
open Normio.Storage.Queries

open Normio.Commands.Api.OpenExam
open Normio.Commands.Api.StartExam
open Normio.Commands.Api.EndExam
open Normio.Commands.Api.CloseExam
open Normio.Commands.Api.AddStudent
open Normio.Commands.Api.RemoveStudent
open Normio.Commands.Api.AddHost
open Normio.Commands.Api.RemoveHost
open Normio.Commands.Api.ChangeTitle

// TODO : JSON parse using Newtonsoft.Json --> System.Text.Json
let handleCommandRequest queries eventStore = function
    | OpenExamRequest exam ->
        openExamCommander
        |> handleCommand eventStore exam
    | StartExamRequest examId ->
        startExamCommander eventStore.GetState
        |> handleCommand eventStore examId
    | EndExamRequest examId ->
        endExamCommander eventStore.GetState
        |> handleCommand eventStore examId
    | CloseExamRequest examId ->
        closeExamCommander eventStore.GetState
        |> handleCommand eventStore examId
    | AddStudentRequest (examId, student) ->
        addStudentCommander eventStore.GetState 
        |> handleCommand eventStore (examId, student)
    | RemoveStudentRequest (examId, studentId) ->
        removeStudentCommander eventStore.GetState
        |> handleCommand eventStore (examId, studentId)
    | AddHostRequest (examId, host) ->
        addHostCommander eventStore.GetState 
        |> handleCommand eventStore (examId, host)
    | RemoveHostRequest (examId, hostId) ->
        removeHostCommander eventStore.GetState
        |> handleCommand eventStore (examId, hostId)
    | ChangeTitleRequest (examId, newTitle) ->
        changeTitleCommander eventStore.GetState
        |> handleCommand eventStore (examId, newTitle)
    | _ -> Error "Invalid Command" |> async.Return
