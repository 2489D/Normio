module Normio.Commands

open Domain

type Command =
  | StartExam of RoomGuid
  | EndExam of RoomGuid
