module Normio.Web.Dev.App

open System
open System.IO
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Cors.Infrastructure
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.SignalR
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection

open Giraffe

open Normio.Storage.InMemory
open Normio.Web.Dev.CommandApiHandler
open Normio.Web.Dev.QueryApiHandler
open Normio.Web.Dev.Configurations
open Normio.Web.Dev.Hub

// ---------------------------------
// Web app
// ---------------------------------

let webApp =
    let eventStore = inMemoryEventStore
    choose [
        commandApi eventStore
        queriesApi inMemoryQueries eventStore
        setStatusCode 404 >=> text "Not Found"
    ]

[<EntryPoint>]
let main args =
    let contentRoot = Directory.GetCurrentDirectory()
    let webRoot     = Path.Combine(contentRoot, "WebRoot")
    Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(
                fun webHostBuilder ->
                    webHostBuilder
                        .UseEnvironment(Environments.Development)
                        .UseContentRoot(contentRoot)
                        .UseWebRoot(webRoot)
                        .Configure(configureApp webApp)
                        .ConfigureServices(configureServices)
                        .ConfigureLogging(configureLogging)
                        |> ignore)
                .Build()
                .Run()
    0