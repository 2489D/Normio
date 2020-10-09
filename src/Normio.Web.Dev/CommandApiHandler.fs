module Normio.Web.Dev.CommandApiHandler

open System.IO
open Microsoft.AspNetCore.Http
open FSharp.Control.Tasks.V2.ContextInsensitive

open Giraffe

open Normio.Core.Events
open Normio.Storage.InMemory
open Normio.Storage.Projections
open Normio.Commands.Api.CommandApi

let eventStream = Event<Event list>()

let project event =
    projectReadModel inMemoryActions event
    |> Async.RunSynchronously |> ignore

let projectEvents events =
    events
    |> List.iter project

let commandApiHandler eventStore : HttpHandler =
    fun (next: HttpFunc) (context : HttpContext) -> task {
        use stream = new StreamReader(context.Request.Body);
        let! payload = stream.ReadToEndAsync();
        let! response = handleCommandRequest inMemoryQueries eventStore payload
        match response with
        | Ok (state, events) ->
            do! inMemoryEventStore.SaveEvents events
            eventStream.Trigger(events)
            return! json state next context
        | Error msg ->
            return! json msg next context
    }

let commandApi eventStore =
    route "/command" >=> POST >=> commandApiHandler eventStore
