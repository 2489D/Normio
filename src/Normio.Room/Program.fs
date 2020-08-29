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
        match evolve initState StartExam with
        | Ok (newState, _) -> newState
        | Error _ -> failwith "EvolveError"  // FIXME
    let finalState =
        match evolve secondState EndExam with
        | Ok (newState, _) -> newState
        | Error _ -> failwith "EvolveError"  // FIXME
    printfn "OK"
    0 // return an integer exit code
