open System
open System.Text
open Suave
open Suave.Filters
open Suave.Operators
open Suave.Successful
open Suave.RequestErrors

open Normio.Domain
open Normio.States
open Normio.Storage.InMemory
open Normio.CommandApi

let commandApiHandler eventStore (context : HttpContext) = async {
    let payload = Encoding.UTF8.GetString context.request.rawForm
    let! response = handleCommandRequest queryInMemory eventStore payload
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
        let eventStore = eventStoreInMemory
        choose [
          commandApi eventStore
        ]
    let cfg =
        {defaultConfig with
            bindings = [HttpBinding.createSimple HTTP "0.0.0.0" 8083]}
    startWebServer cfg app
    0