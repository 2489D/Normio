﻿module Normio.Web.Program

open System
open System.Text
open Suave
open Suave.Filters
open Suave.Operators
open Suave.Successful
open Suave.RequestErrors

open Normio.Core.Domain
open Normio.Core.States
open Normio.Core.Events
open Normio.Storage.InMemory
open Normio.Storage.Projections
open Normio.Commands.Api.CommandApi
open Normio.Web.JsonFormatter

let eventStream = Event<Event list>()
let project event =
    projectReadModel inMemoryActions event
    |> Async.RunSynchronously |> ignore

let projectEvents events = events |> List.iter project

let commandApiHandler eventStore (context : HttpContext) = async {
    let payload = Encoding.UTF8.GetString context.request.rawForm
    let! response = handleCommandRequest inMemoryQueries eventStore payload
    match response with
    | Ok (state, events) ->
        do! eventStoreObj.SaveEvents events
        eventStream.Trigger(events)
        return! toStateJson state context
    | Error msg ->
        return! toErrorJson msg context
}

let commandApi eventStore =
    path "/command"
        >=> POST
        >=> commandApiHandler eventStore

[<EntryPoint>]
let main argv =
    printfn "---Normio Web Server Starts---"
    let app =
        let eventStore = eventStoreObj
        choose [
          commandApi eventStore
        ]
    do eventStream.Publish.Add(projectEvents)
    do eventStream.Publish.Add(printfn "%A")
    let cfg =
        {defaultConfig with
            bindings = [HttpBinding.createSimple HTTP "0.0.0.0" 8083]}
    startWebServer cfg app
    0