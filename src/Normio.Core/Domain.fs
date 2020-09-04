module Normio.Domain

open System

type RoomTitle40 = RoomTitle40 of string

type Room = {
    Id: Guid
    Title: RoomTitle40 option
}

module Room =
    let init =
        {
            Id = Guid.NewGuid()
            Title = None
        }

module RoomTitle40 =
    let tryOfString s =
        if s |> String.length > 40
        then Some (RoomTitle40 s)
        else None

    let toString (RoomTitle40 title) = title
