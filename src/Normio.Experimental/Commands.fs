module Normio.Commands

open Domain

// TODO: Do we need RoomGuid in Command?
type Command =
  | StartExam of RoomGuid
  | EndExam of RoomGuid
