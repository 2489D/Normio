module Normio.Web.Dev.App

open System
open System.Configuration
open System.IO
open System.Net.Http
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Cors.Infrastructure
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.SignalR
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection

open Giraffe

open Normio.Storage.Exams
open Normio.Storage.EventStore
open Normio.Storage.EventStore.Cosmos
open Normio.Web.Dev.CommandApiHandler
open Normio.Web.Dev.QueryApiHandler
open Normio.Web.Dev.Configurations
open Normio.Web.Dev.Hub

// ---------------------------------
// Web app
// ---------------------------------

open Microsoft.Extensions.Configuration

open FSharp.Control.Tasks.V2.ContextInsensitive


let webApp =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        let settings = ctx.GetService<IConfiguration>()
        let conn = settings.["EventStoreConnString"]
        printfn "%s" conn
        let eventStore = cosmosEventStore conn
        task {
            return! choose [
                commandApi eventStore
                queriesApi inMemoryQueries eventStore
                setStatusCode 404 >=> text "Not Found"
            ] next ctx
        }

[<EntryPoint>]
let main args =
    let contentRoot = Directory.GetCurrentDirectory()
    let webRoot     = Path.Combine(contentRoot, "WebRoot")
    Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(fun webHostBuilder ->
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