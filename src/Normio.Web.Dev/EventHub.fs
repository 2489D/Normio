module Normio.Web.Dev.Hub

open System.Threading.Tasks
open Microsoft.AspNetCore.SignalR
open FSharp.Control.Tasks.V2.ContextInsensitive

open Microsoft.Extensions.Configuration
open Normio.Core.Events
open Normio.Persistence.Projections
open Normio.Persistence.Exams


type INormioClient =
    abstract member ReceiveEvent : Event -> Task

type EventHub() =
    inherit Hub<INormioClient>()

type NormioEventWorker(hubContext: IHubContext<EventHub, INormioClient>, config: IConfiguration) =
    let eventStream = Event<Event list>()
    let conn = config.["EventStoreConnString"]

    let project actions event =
        projectReadModel actions event
        |> Async.RunSynchronously |> ignore

    let projectEvents actions =
        List.iter (project actions)

    // TODO : user group
    let signalEvents (events: Event list) =
        task {
            for event in events do
                do! hubContext.Clients.All.ReceiveEvent event
        } |> ignore
        ()

    do eventStream.Publish.Add(projectEvents (cosmosActions conn))
    do eventStream.Publish.Add(signalEvents)
    do eventStream.Publish.Add(printfn "Events created : %A")

    member this.Trigger events =
        eventStream.Trigger events

