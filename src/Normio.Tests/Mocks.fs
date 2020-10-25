module Normio.Tests.Mocks

open System
open Normio.Core.Domain

let newGuid () = Guid.NewGuid ()

let validTitle =
    match "valid title" |> ExamTitle40.create with
    | Ok title -> title
    | Error _ -> failwith "this should be a valid title"

let validName =
    match "valid name" |> UserName40.create with
    | Ok name -> name
    | Error _ -> failwith "this should be valid name"

let validFileName =
    match "valid file string" |> FileString200.create with
    | Ok s -> s
    | Error _ -> failwith "this should be a valid string"

let aStudent: Student =
    { Id = newGuid ()
      Name = validName }
    
let aHost: Host =
     { Id = newGuid ()
       Name = validName }

let aFile: File =
    { Id = newGuid ()
      FileName = validFileName }

let mockStudents =
    [(newGuid (), aStudent)] |> Map.ofList
    
let mockHosts =
    [(newGuid (), aHost)] |> Map.ofList

let mockFiles = [aFile]
