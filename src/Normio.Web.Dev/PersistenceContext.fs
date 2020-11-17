[<AutoOpen>]
module Normio.Web.Dev.PersistenceContext

open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Configuration

open Normio.Persistence.EventStore.InMemory
open Normio.Persistence.ReadModels.InMemory


let getEventStore (config: IConfiguration) (env: IWebHostEnvironment) =
    let envName = env.EnvironmentName
    match envName with
    | "Development" -> inMemoryEventStore
    | "Staging" -> inMemoryEventStore
    | "Production" -> inMemoryEventStore
    | _ -> failwith "Cannot determine running environment"

let getQuerySide (config: IConfiguration) (env: IWebHostEnvironment) =
    let envName = env.EnvironmentName
    match envName with
    | "Development" -> inMemoryQueries
    | "Staging" -> inMemoryQueries
    | "Production" -> inMemoryQueries
    | _ -> failwith "Cannot determine running environment"

let getActions (config: IConfiguration) (env: IWebHostEnvironment) =
    let envName = env.EnvironmentName
    match envName with
    | "Development" -> inMemoryActions
    | "Staging" -> inMemoryActions
    | "Production" -> inMemoryActions
    | _ -> failwith "Cannot determine running environment"

