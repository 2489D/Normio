module Normio.Tests.Mocks

open System
open Normio.Core

let newGuid () = Guid.NewGuid ()

let validTitle =
    match "valid title" |> ExamTitle40.create with
    | Ok title -> title
    | Error _ -> failwith "this should be a valid title"

let validName =
    match "valid name" |> UserName40.create with
    | Ok name -> name
    | Error _ -> failwith "this should be valid name"

let validStartTime = DateTime.MaxValue
let validDuration = TimeSpan.MaxValue

let aStudent: Student =
    { Id = newGuid ()
      Name = validName }
    
let aHost: Host =
     { Id = newGuid ()
       Name = validName }
 
let mockStudents =
    [(newGuid (), aStudent)] |> Map.ofList
    
let mockHosts =
    [(newGuid (), aHost)] |> Map.ofList
