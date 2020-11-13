module Normio.Web.Dev.App

open System.IO
open Microsoft.Extensions.Hosting
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http

open Giraffe

open Normio.Persistence.FileSave
open Normio.Web.Dev.CommandApiHandler
open Normio.Web.Dev.QueryApiHandler
open Normio.Web.Dev.Configurations

open Microsoft.Extensions.Configuration

open FSharp.Control.Tasks.V2.ContextInsensitive

let webApp =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        let env = ctx.GetService<IWebHostEnvironment>()
        let config = ctx.GetService<IConfiguration>()
        let eventStore = getEventStore config env
        let queries = getQuerySide config env
        let fileSaver = inMemoryFileSaver """/Users/bonjune/2489D/Normio/src"""
        task {
            return! choose [
                commandApi eventStore fileSaver
                queriesApi queries
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