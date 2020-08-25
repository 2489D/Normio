module Normio.Events

open Domain

type Event =
  | ExamStarted of RoomGuid
  | ExamEnded of RoomGuid
