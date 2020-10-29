module Normio.Web.Dev.CommandApiHandler

open System.IO
open Microsoft.AspNetCore.Http
open FSharp.Control.Tasks.V2.ContextInsensitive

open Giraffe

open Normio.Persistence.EventStore
open Normio.Commands.Api.CommandApi
open Normio.Web.Dev.Hub

// TODO : change to RESTful api implementation

// TODO : status code
let commandApiHandler queries (eventStore : IEventStore) : HttpHandler =
    fun (next: HttpFunc) (ctx : HttpContext) -> task {
        let eventHub = ctx.GetService<NormioEventWorker>()
        use stream = new StreamReader(ctx.Request.Body)
        let! payload = stream.ReadToEndAsync()
        let! response = handleCommandRequest queries eventStore payload
        // TODO : better way? --> https://github.com/Tarmil/FSharp.SystemTextJson/blob/master/docs/Using.md#using-with-giraffe
        match response with
        | Ok (state, events) ->
            do! eventStore.SaveEvents events
            eventHub.Trigger events
            return! json state next ctx
        | Error msg ->
            return! (setStatusCode 404 >=> json msg) next ctx
    }

let commandApi queries eventStore =
    route "/command" >=> POST >=> commandApiHandler queries eventStore
