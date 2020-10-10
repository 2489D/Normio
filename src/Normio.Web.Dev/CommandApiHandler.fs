module Normio.Web.Dev.CommandApiHandler

open System.Threading.Tasks
open System.IO
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.SignalR
open FSharp.Control.Tasks.V2.ContextInsensitive

open Giraffe

open Normio.Core.Events
open Normio.Storage.InMemory
open Normio.Storage.Projections
open Normio.Commands.Api.CommandApi
open Normio.Web.Dev.Hub
open Normio.Web.Dev.JsonFormatter

let commandApiHandler eventStore : HttpHandler =
    fun (next: HttpFunc) (context : HttpContext) -> task {
        let eventHub = context.GetService<NormioEventService>()
        use stream = new StreamReader(context.Request.Body);
        let! payload = stream.ReadToEndAsync();
        let! response = handleCommandRequest inMemoryQueries eventStore payload
        match response with
        | Ok (state, events) ->
            do! inMemoryEventStore.SaveEvents events
            eventHub.Trigger events
            return! json state next context
        | Error msg ->
            return! json msg next context
    }

let commandApi eventStore =
    route "/command" >=> POST >=> commandApiHandler eventStore
