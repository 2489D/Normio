module Normio.States

open System
open Domain
open Events

/// Initial state: RoomIsWaiting None
type State =
    | RoomIsClosed of Guid option
    | RoomIsWaiting of Room
    | RoomOnExam of Room
    | RoomExamFinished of Room

let apply state event =
    match state, event with
    | RoomIsClosed None, RoomOpened (id, title) ->
        RoomIsWaiting {
            Id = id
            Title = Some title
        }
    | RoomIsWaiting room, ExamStarted _ ->
        RoomOnExam room
    | RoomOnExam room, ExamEnded _ ->
        RoomExamFinished room
    | RoomExamFinished room, RoomClosed _ ->
        RoomIsClosed (Some room.Id)
    | _ -> state

