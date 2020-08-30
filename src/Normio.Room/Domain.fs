module Normio.Domain

open System

type RoomGuid = Guid
type RoomTitle40 = RoomTitle40 of string

type Room = {
    Title: RoomTitle40
}

module Room =
    let init title =
        {
            Title = title 
        }

module RoomTitle40 =
    let create title =
        if title |> String.length > 40 then None
        else Some (RoomTitle40 title)

    let toString (RoomTitle40 title) = title
