module Normio.Web.Dev.CommandApiHandler

open System.IO
open Microsoft.AspNetCore.Http
open FSharp.Control.Tasks.V2.ContextInsensitive

open Giraffe

open Normio.Storage.EventStore
open Normio.Storage.Exams
open Normio.Commands.Api.CommandApi
open Normio.Web.Dev.Hub

// TODO : status code
let commandApiHandler (eventStore : IEventStore) : HttpHandler =
    fun (next: HttpFunc) (context : HttpContext) -> task {
        let eventHub = context.GetService<NormioEventWorker>()
        use stream = new StreamReader(context.Request.Body);
        let! payload = stream.ReadToEndAsync();
        let! response = handleCommandRequest inMemoryQueries eventStore payload
        // TODO : better way? --> https://github.com/Tarmil/FSharp.SystemTextJson/blob/master/docs/Using.md#using-with-giraffe
        match response with
        | Ok (state, events) ->
            do! eventStore.SaveEvents events
            eventHub.Trigger events
            return! json state next context
        | Error msg ->
            return! (setStatusCode 404 >=> json msg) next context
    }

let commandApi eventStore =
    route "/command" >=> POST >=> commandApiHandler eventStore
