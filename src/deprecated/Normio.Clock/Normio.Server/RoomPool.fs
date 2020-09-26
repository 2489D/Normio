module Normio.Server.RoomPool

open System
open Normio.States
open Normio.Domain

type RoomWorker =
    | Running of Guid * State
    | Free

module RoomWorker =
    let run = function
        | Free -> Running (Guid.NewGuid(), RoomIsWaiting (Room.init (RoomTitle40 "default"))) |> Ok
        | Running _ -> "The room is already running" |> Error

    let free = function
        | Running _ -> Free |> Ok
        | Free -> "The room is already freed" |> Error 

type RoomPool = RoomWorker list

module RoomPool =
    let init size = List.init size (fun _ -> Free)
    let findFree pool =
        let freeRoom = pool |> List.tryFind (function | Free -> true | _ -> false) 
        match freeRoom with
        | Some room -> room |> Ok
        | None -> "Cannot find a free room" |> Error


