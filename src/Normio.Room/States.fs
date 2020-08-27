module Normio.States

open Domain
open Events

type State =
    | RoomIsWaiting of Room
    | RoomOnExam of Room
    | RoomExamFinished of Room
    | RoomIsClosed

let apply state event =
    match state, event with
    // Events during RoomIsWaiting
    | RoomIsWaiting room, ExamStarted _ ->
        RoomOnExam room

    // Events during RoomOnExam
    | RoomOnExam room, ExamEnded _ ->
        RoomExamFinished room

    | _ -> state
