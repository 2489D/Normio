module Normio.Projections

open System
open Normio.Events
open Normio.Domain

type RoomActions = {
    OpenRoom: Guid -> RoomTitle40 -> Async<unit>
    StartExam: Guid -> Async<unit>
    EndExam: Guid -> Async<unit>
    CloseRoom: Guid -> Async<unit>
}

type StudentActions = {
    Room: RoomActions
}

type HostActions = {
    Room: RoomActions
}

type ProjectionActions = {
    Student: StudentActions
    Host: HostActions
}

let projectReadModel actions = function
| RoomOpened (id, title) ->
    [
        actions.Student.Room.OpenRoom id title
        actions.Host.Room.OpenRoom id title
    ] |> Async.Parallel
| ExamStarted guid ->
    [
        actions.Student.Room.StartExam guid
        actions.Host.Room.StartExam guid
    ] |> Async.Parallel
| ExamEnded guid ->
    [
        actions.Student.Room.EndExam guid
        actions.Host.Room.EndExam guid
    ] |> Async.Parallel
| RoomClosed guid ->
    [
        actions.Student.Room.CloseRoom guid
        actions.Host.Room.CloseRoom guid
    ] |> Async.Parallel
