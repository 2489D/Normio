module Normio.Domain

open System

type RoomGuid = Guid
type RoomTitle40 = RoomTitle40 of string

type Room = {
  Id: RoomGuid
  Title: RoomTitle40
}

module Room =
  let init title = {
    Id = RoomGuid.NewGuid ()
    Title = title
  }

module RoomTitle40 =
  let create title =
    if title |> String.length > 40 then None
    else Some (RoomTitle40 title)
