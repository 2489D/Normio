module Normio.Web.Dev.Hub

open System.Threading.Tasks
open Microsoft.Extensions.DependencyInjection
open Microsoft.AspNetCore.SignalR

open Normio.Core.Events
open Normio.Storage.Projections
open Normio.Storage.InMemory

open FSharp.Control.Tasks.V2.ContextInsensitive

type INormioClientApi =
    abstract member ReceiveEvent : Event -> Task

type EventHub() =
    inherit Hub<INormioClientApi>()

    member this.SendEvent event = task {
        do! this.Clients.All.ReceiveEvent event
    }

type NormioEventService(hubContext: IHubContext<EventHub, INormioClientApi>) =
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
                do! hubContext.Clients.All.ReceiveEvent event
        } |> ignore
        ()

    do eventStream.Publish.Add(projectEvents)
    do eventStream.Publish.Add(signalEvents)

    member this.Trigger events =
        eventStream.Trigger events

