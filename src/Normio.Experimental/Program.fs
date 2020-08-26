module Normio.Program

open Domain
open Commands
open States
open CommandHandlers

[<EntryPoint>]
let main argv =
  let exampleRoomTitle =
    match RoomTitle40.create "Example Room Name" with
    | Some title -> title
    | None -> failwith "RoomTitleError"  // FIXME
  let exampleRoom = Room.init exampleRoomTitle
  let initState = RoomIsWaiting exampleRoom
  let secondState =
    match evolve initState (StartExam exampleRoom.Id) with
    | Ok (newState, events) -> newState
    | Error _ -> failwith "EvolveError"  // FIXME
  let finalState =
    match evolve secondState (EndExam exampleRoom.Id) with
    | Ok (newState, events) -> newState
    | Error _ -> failwith "EvolveError"  // FIXME
  0 // return an integer exit code
