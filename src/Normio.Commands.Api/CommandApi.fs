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

// TODO: 
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
    | _ -> Error "Invalid Command" |> async.Return
