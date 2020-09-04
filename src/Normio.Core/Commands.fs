module Normio.Commands

open System
open Domain

type Command =
    | OpenRoom of Guid * RoomTitle40
    | StartExam of Guid
    | EndExam of Guid
    | CloseRoom of Guid

