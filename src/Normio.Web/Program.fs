open System
open System.Text
open Suave
open Suave.Filters
open Suave.Operators
open Suave.Successful
open Suave.RequestErrors

open Normio.Core.Domain
open Normio.Core.States
open Normio.Storage.InMemory
open Normio.Commands.Api.CommandApi

let commandApiHandler eventStore (context : HttpContext) = async {
    let payload = Encoding.UTF8.GetString context.request.rawForm
    let! response = handleCommandRequest inMemoryQueries eventStore payload
    match response with
    | Ok (state, events) ->
        return! OK (sprintf "%A" state) context
    | Error msg ->
        return! BAD_REQUEST msg context
}

let commandApi eventStore =
    path "/command"
        >=> POST
        >=> commandApiHandler eventStore

[<EntryPoint>]
let main argv =
    let app =
        let eventStore = inMemoryEventStore
        choose [
          commandApi eventStore
        ]
    let cfg =
        {defaultConfig with
            bindings = [HttpBinding.createSimple HTTP "0.0.0.0" 8083]}
    startWebServer cfg app
    0