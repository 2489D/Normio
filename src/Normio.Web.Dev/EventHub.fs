module Normio.Web.Dev.Hub

open System.Threading.Tasks
open Microsoft.Extensions.Hosting
open Microsoft.AspNetCore.SignalR

open Normio.Core.Events
open Normio.Storage.Projections
open Normio.Storage.InMemory

open FSharp.Control.Tasks.V2.ContextInsensitive

type INormioClient =
    abstract member ReceiveEvent : string -> Task

type EventHub() =
    inherit Hub<INormioClient>()

type NormioEventWorker(hubContext: IHubContext<EventHub, INormioClient>) =
    let eventStream = new Event<Event list>()

    let project event =
        projectReadModel inMemoryActions event
        |> Async.RunSynchronously |> ignore

    let projectEvents events =
        events
        |> List.iter project

    let signalEvents (events: Event list) =
        task {
            for event in events do
                do! hubContext.Clients.All.ReceiveEvent (string event)
        } |> ignore
        ()

    do eventStream.Publish.Add(projectEvents)
    do eventStream.Publish.Add(signalEvents)
    do eventStream.Publish.Add(printfn "Events created : %A")

    member this.Trigger events =
        eventStream.Trigger events

