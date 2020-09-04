module Normio.CommandHandlers

open Events
open Commands
open Errors
open States

let handleStartExam = function
    | RoomIsWaiting room ->
        [ExamStarted room.Id] |> Ok
    | _ -> Error ExamAlreadyStarted

let handleEndExam = function
    | RoomOnExam room ->
        [ExamEnded room.Id] |> Ok
    | _ -> Error CannotEndExam

let handleCloseRoom = function
    | RoomIsWaiting room ->
        [RoomClosed room.Id] |> Ok
    | RoomExamFinished room ->
        [RoomClosed room.Id] |> Ok
    | _ -> Error CannotCloseRoom

let handleOpenRoom id title = function
    | RoomIsClosed None ->
        [RoomOpened (id, title)] |> Ok
    | _ -> Error RoomAlreadyOpened

let execute state = function
    | OpenRoom (id, title) -> handleOpenRoom id title state
    | StartExam _ -> handleStartExam state
    | EndExam _ -> handleEndExam state
    | CloseRoom _ -> handleCloseRoom state

let evolve state command =
    match execute state command with
    | Ok newEvents ->
        let newState = List.fold apply state newEvents 
        (newState, newEvents) |> Ok
    | Error err -> Error err

