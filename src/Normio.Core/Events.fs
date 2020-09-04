module Normio.Events

open System
open Domain

type Event =
    | RoomOpened of Guid * RoomTitle40
    | ExamStarted of Guid
    | ExamEnded of Guid
    | RoomClosed of Guid
