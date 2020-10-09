module Normio.Web.Dev.Hub

open Microsoft.AspNetCore.SignalR

type EventHub() =
    inherit Hub()
    member this.SendEvent eventJson =
        this.Clients.All.SendAsync(eventJson)
