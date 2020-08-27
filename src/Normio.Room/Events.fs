module Normio.Events

open Domain

// TODO: Do we need RoomGuid in Event?
type Event =
    | ExamStarted of RoomGuid
    | ExamEnded of RoomGuid
