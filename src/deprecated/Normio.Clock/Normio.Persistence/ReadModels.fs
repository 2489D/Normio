module Normio.ReadModels

open System
open Normio.Domain

(*
ReadModel definitions to answer the queries.
We need to persist read models in a storage & retrieve to answer those queries.
*)

(*
Leveraging `event` returned from the `evolve` function,
we can populate our read models.
*)

// Normio.Core.States contains `Room`
// where is not good for use here
// since that exposes too much information to users
// We define here just for the users
type RoomStatusAsUser =
| RoomIsWaiting of Guid
| RoomOnExam of Guid
| RoomExamFinished of Guid
| RoomIsClosed

type RoomAsStudent = {
    Title: RoomTitle40
    Status: RoomStatusAsUser
}

type RoomAsHost  = {
    Title: RoomTitle40
    Status: RoomStatusAsUser
}