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

let handleCloseRoom = function
    | RoomIsWaiting _ 
    | RoomExamFinished _ ->
        [RoomClosed] |> Ok
    | _ -> Error CannotCloseRoom

let execute state = function
    | StartExam _ -> handleStartExam state
    | EndExam _ -> handleEndExam state
    | CloseRoom _ -> handleCloseRoom state

let evolve state command =
    match execute state command with
    | Ok newEvents ->
        let newState = List.fold apply state newEvents 
        (newState, newEvents) |> Ok
    | Error err -> Error err

