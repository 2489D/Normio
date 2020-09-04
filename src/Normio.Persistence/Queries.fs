(* Exposing the projection to the outside world *)
module Normio.Queries

open System
open Normio.Domain
open Normio.ReadModels

type StudentQueries = {
    GetRoom: Guid -> Async<RoomAsStudent>
}

type HostQueries = {
    GetRoom: Guid -> Async<RoomAsHost>
}

type Queries = {
    Student: StudentQueries
    Host: HostQueries
}