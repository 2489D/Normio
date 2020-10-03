module Normio.CommandApi

open System.Text
open Normio.CommandHandlers
open Normio.Commands.Api.OpenExam
open Normio.Queries

let handleCommandRequest validationQueries eventStore
    = function
    | OpenExamRequest title ->
        openExamCommander
        |> handleCommand eventStore title
    | _ -> Error "Invalid Command" |> async.Return