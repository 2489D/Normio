module Normio.EventStore

open System

open Normio.Domain
open Normio.States
open Normio.Events

open NEventStore

// p.139 from the book
let getStateFromEvents events =
    events
    |> Seq.fold apply (RoomIsClosed None)

let getRoomIdFromState = function
| RoomIsWaiting room -> Some room.Id
| RoomOnExam room -> Some room.Id
| RoomExamFinished room -> Some room.Id
| RoomIsClosed (Some id) -> Some id
| RoomIsClosed None -> None

let saveEvent (storeEvents: IStoreEvents) state event =
    let roomId = getRoomIdFromState state
    use stream = storeEvents.OpenStream(roomId.ToString())
    stream.Add(new EventMessage(Body = event))
    stream.CommitChanges(Guid.NewGuid())

let saveEvents (storeEvents: IStoreEvents) state events =
    events
    |> List.iter ( saveEvent storeEvents state)
    |> async.Return

let getEvents (storeEvents: IStoreEvents) (roomId: Guid) =
    use stream = storeEvents.OpenStream(roomId.ToString())
    stream.CommittedEvents
    |> Seq.map (fun msg -> msg.Body)
    |> Seq.cast<Event>

let getState storeEvents roomId =
    getEvents storeEvents roomId
    |> Seq.fold apply (RoomIsClosed None)
    |> async.Return

type EventStore = {
    GetState: Guid -> Async<State>
    SaveEvent: State -> Event -> Async<unit>
}