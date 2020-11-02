module Normio.Web.Dev.App

open System.IO
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Hosting

open Giraffe

open Normio.Persistence.Exams
open Normio.Persistence.EventStore.Cosmos
open Normio.Web.Dev.CommandApiHandler
open Normio.Web.Dev.QueryApiHandler
open Normio.Web.Dev.Configurations

open Microsoft.Extensions.Configuration

open FSharp.Control.Tasks.V2.ContextInsensitive


let webApp =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        let settings = ctx.GetService<IConfiguration>()
        let conn = settings.["EventStoreConnString"]

        let eventStore = cosmosEventStore conn
        let queries = cosmosQueries conn
        task {
            return! choose [
                commandApi eventStore
                queriesApi queries eventStore
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