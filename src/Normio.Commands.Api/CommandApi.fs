module Normio.Commands.Api.CommandApi

open System
open System.Text
open Normio.Core.States
open Normio.Core.Events
open Normio.Commands.Api.CommandHandlers
open Normio.Storage.Queries

open Normio.Commands.Api.OpenExam
open Normio.Commands.Api.StartExam

let handleCommandRequest queries eventStore
    = function
    | OpenExamRequest exam ->
        openExamCommander
        |> handleCommand eventStore exam
    | StartExamRequest examId ->
        startExamCommander queries.Exam.GetExam
        |> handleCommand eventStore examId
    | _ -> Error "Invalid Command" |> async.Return
