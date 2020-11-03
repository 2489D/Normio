[<AutoOpen>]
module Normio.Web.Dev.PersistenceContext

open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Configuration

open Normio.Persistence.EventStore.InMemory
open Normio.Persistence.EventStore.Cosmos
open Normio.Persistence.ReadModels.InMemory
open Normio.Persistence.ReadModels.Cosmos


let getEventStore (config: IConfiguration) (env: IWebHostEnvironment) =
    let envName = env.EnvironmentName
    match envName with
    | "Development" | "Staging" ->
        inMemoryEventStore
    | "Production" ->
        let conn = config.["EventStoreConnString"]
        cosmosEventStore conn
    | _ ->
        failwith "Cannot determine running environment"

let getQuerySide (config: IConfiguration) (env: IWebHostEnvironment) =
    let envName = env.EnvironmentName
    match envName with
    | "Development" | "Staging" ->
        inMemoryQueries
    | "Production" ->
        let conn = config.["EventStoreConnString"]
        cosmosQueries conn
    | _ ->
        failwith "Cannot determine running environment"

let getActions (config: IConfiguration) (env: IWebHostEnvironment) =
    let envName = env.EnvironmentName
    match envName with
    | "Development" | "Staging" ->
        inMemoryActions
    | "Production" ->
        let conn = config.["EventStoreConnString"]
        cosmosActions conn
    | _ ->
        failwith "Cannot determine running environment"

