module Normio.CommandApi

open System
open System.Text
open Normio.States
open Normio.Events
open Normio.CommandHandlers
open Normio.Queries

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
