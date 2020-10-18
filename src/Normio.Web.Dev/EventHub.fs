module Normio.Web.Dev.Hub

open System.Threading.Tasks
open Microsoft.AspNetCore.SignalR
open FSharp.Control.Tasks.V2.ContextInsensitive

open Normio.Core.Events
open Normio.Storage.Projections
open Normio.Storage.Exams


type INormioClient =
    abstract member ReceiveEvent : string -> Task

type EventHub() =
    inherit Hub<INormioClient>()

type NormioEventWorker(hubContext: IHubContext<EventHub, INormioClient>) =
    let eventStream = new Event<Event list>()

    let project actions event =
        projectReadModel actions event
        |> Async.RunSynchronously |> ignore

    let projectEvents actions =
        List.iter (project actions)

    let signalEvents (events: Event list) =
        task {
            for event in events do
                do! hubContext.Clients.All.ReceiveEvent (string event)
        } |> ignore
        ()

    do eventStream.Publish.Add(projectEvents inMemoryActions)
    do eventStream.Publish.Add(signalEvents)
    do eventStream.Publish.Add(printfn "Events created : %A")

    member this.Trigger events =
        eventStream.Trigger events

