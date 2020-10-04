module Normio.CommandApi

open System.Text
open Normio.States
open Normio.Events
open Normio.CommandHandlers
open Normio.Commands.Api.OpenExam
open Normio.Queries

let handleCommandRequest queries eventStore
    = function
    | OpenExamRequest exam ->
        openExamCommander
        |> handleCommand eventStore exam
    | StartExamRequest examId ->
        startExamCommander queries.Exam.GetExam
        |> handleCommand eventStore examId
    | _ -> Error "Invalid Command" |> async.Return
