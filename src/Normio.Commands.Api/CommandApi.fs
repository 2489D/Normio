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
open Normio.Commands.Api.CreateSubmission
open Normio.Commands.Api.CreateQuestion
open Normio.Commands.Api.DeleteQuestion
open Normio.Commands.Api.ChangeTitle

let handleCommandRequest queries eventStore = function
    | OpenExamRequest (examId, title) ->
        openExamCommander
        |> handleCommand eventStore (examId, title)
    | StartExamRequest examId ->
        startExamCommander queries.Exam.GetExamByExamId
        |> handleCommand eventStore examId
    | EndExamRequest examId ->
        endExamCommander queries.Exam.GetExamByExamId
        |> handleCommand eventStore examId
    | CloseExamRequest examId ->
        closeExamCommander queries.Exam.GetExamByExamId
        |> handleCommand eventStore examId
    | AddStudentRequest (examId, studentId, name) ->
        addStudentCommander queries.Exam.GetExamByExamId 
        |> handleCommand eventStore (examId, studentId, name)
    | RemoveStudentRequest (examId, studentId) ->
        removeStudentCommander queries.Exam.GetExamByExamId
        |> handleCommand eventStore (examId, studentId)
    | AddHostRequest (examId, hostId, name) ->
        addHostCommander queries.Exam.GetExamByExamId 
        |> handleCommand eventStore (examId, hostId, name)
    | CreateSubmissionRequest req ->
        createSubmissionCommander queries.Exam.GetExamByExamId
        |> handleCommand eventStore req
    | CreateQuestionRequest (examId, (fileId, fileName)) ->
        createCreateQuestionCommander queries.Exam.GetExamByExamId
        |> handleCommand eventStore (examId, fileId, fileName)
    | DeleteQuestionRequest (examId, fileId) ->
        createDeleteQuestionCommander queries.Exam.GetExamByExamId
        |> handleCommand eventStore (examId, fileId)
    | RemoveHostRequest (examId, hostId) ->
        removeHostCommander queries.Exam.GetExamByExamId
        |> handleCommand eventStore (examId, hostId)
    | ChangeTitleRequest (examId, newTitle) ->
        changeTitleCommander queries.Exam.GetExamByExamId
        |> handleCommand eventStore (examId, newTitle)
    | _ -> Error "Invalid Command" |> async.Return
