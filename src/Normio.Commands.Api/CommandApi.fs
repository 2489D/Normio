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
        handleCommand eventStore exam openExamCommander
    | _ -> Error "Invalid Command" |> async.Return
