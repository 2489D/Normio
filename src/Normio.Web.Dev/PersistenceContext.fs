[<AutoOpen>]
module Normio.Web.Dev.PersistenceContext

open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Configuration

open Normio.Persistence.EventStore.InMemory
open Normio.Persistence.ReadModels.InMemory
open Normio.Persistence.FileSave

let fileRoot = """/Users/bonjune/2489D/Normio/src""" // TODO

let getEventStore (config: IConfiguration) (env: IWebHostEnvironment) =
    let envName = env.EnvironmentName
    match envName with
    | "Development" -> inMemoryEventStore
    | "Staging" -> inMemoryEventStore
    | "Production" -> inMemoryEventStore
    | _ -> failwith "Cannot determine running environment"

let getFileSaver (config: IConfiguration) (env: IWebHostEnvironment) =
    let envName = env.EnvironmentName
    match envName with
    | "Development" -> inMemoryFileSaver fileRoot
    | "Staging" -> inMemoryFileSaver fileRoot
    | "Production" -> inMemoryFileSaver fileRoot
    | _ -> failwith "Cannot determine running environment"

let getFileGetter (config: IConfiguration) (env: IWebHostEnvironment) =
    let envName = env.EnvironmentName
    match envName with
    | "Development" -> inMemoryFileGetter fileRoot
    | "Staging" -> inMemoryFileGetter fileRoot
    | "Production" -> inMemoryFileGetter fileRoot
    | _ -> failwith "Cannot determine running environment"

let getFileDeleter (config: IConfiguration) (env: IWebHostEnvironment) =
    let envName = env.EnvironmentName
    match envName with
    | "Development" -> inMemoryFileDeleter fileRoot
    | "Staging" -> inMemoryFileDeleter fileRoot
    | "Production" -> inMemoryFileDeleter fileRoot
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

