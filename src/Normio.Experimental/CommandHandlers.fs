module Normio.CommandHandlers

open Domain
open Events
open Commands
open Errors
open States

let handleStartExam = function
    | RoomIsWaiting room ->
        [ExamStarted (room.Id)] |> Ok
    | _ -> Error ExamAlreadyStarted

let handleEndExam = function
    | RoomOnExam room ->
        [ExamEnded room.Id] |> Ok
    | _ -> Error CannotEndExam

let execute state = function
    | StartExam _ -> handleStartExam state
    | EndExam _ -> handleEndExam state

let evolve state command =
    match execute state command with
    | Ok events ->
        let newState = List.fold apply state events
        (newState, events) |> Ok
    | Error err -> Error err
