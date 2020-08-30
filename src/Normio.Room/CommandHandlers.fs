module Normio.CommandHandlers

open Events
open Commands
open Errors
open States

let handleStartExam = function
    | RoomIsWaiting _ ->
        [ExamStarted] |> Ok
    | _ -> Error ExamAlreadyStarted

let handleEndExam = function
    | RoomOnExam _ ->
        [ExamEnded] |> Ok
    | _ -> Error CannotEndExam

let execute state = function
    | StartExam -> handleStartExam state
    | EndExam -> handleEndExam state

let evolve state command =
    match execute state command with
    | Ok events ->
        let newState = List.fold apply state events
        (newState, events) |> Ok
    | Error err -> Error err
