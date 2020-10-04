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
        do! inMemoryEventStore.SaveEvents events
        eventStream.Trigger(events)
        return! OK (sprintf "%A" state) context
    | Error msg ->
        return! BAD_REQUEST msg context
}

let commandApi eventStore =
    path "/command"
        >=> POST
        >=> commandApiHandler eventStore

[<EntryPoint>]
let main argv =
    let app =
        let eventStore = inMemoryEventStore
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