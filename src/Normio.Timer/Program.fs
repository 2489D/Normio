module Normio.Timer.App

open System
open System.Runtime.Serialization
open FsHttp
open Suave
open Suave.Json
open Suave.Filters
open Suave.Operators
open Suave.Successful
open Suave.RequestErrors

open Normio.Core.Commands

let postCommandHandler backendUrl command =
    async {
        match command with
        | StartExam examId ->
            httpAsync {
                POST (backendUrl + "startExam")
                body
                json (sprintf """{ "examId" : %A }""" examId)
            } |> ignore
        | EndExam examId ->
            httpAsync {
                POST (backendUrl + "endExam")
                body
                json (sprintf """{ "examId" : %A }""" examId)
            } |> ignore
        | _ -> () // TODO
    }

let tryParseRawCommand (rawCommand: string) =
    let stringArray = rawCommand.Split(' ')
    match stringArray.[0] with
    | "StartExam" ->
        if stringArray.Length <> 2 then None
        else StartExam (Guid.Parse stringArray.[1]) |> Some
    | "EndExam" ->
        if stringArray.Length <> 2 then None
        else EndExam (Guid.Parse stringArray.[1]) |> Some
    | _ -> None // TODO

[<DataContract>]
type CreateRequest =
    {
        [<field: DataMember(Name = "command")>]
        RawCommand : string
        [<field: DataMember(Name = "time")>]
        RawTime : string
    }

let handleCreateRequest (timer: ITimer): WebPart =
    fun (ctx : HttpContext) ->
        async {
            let req = ctx.request.rawForm |> fromJson<CreateRequest>
            match req.RawCommand |> tryParseRawCommand with
            | Some command ->
                do! timer.CreateTimer command (req.RawTime |> DateTime.Parse)
                return! CREATED (sprintf "command %s at %A created" req.RawCommand req.RawTime) ctx
            | None -> return! NOT_FOUND (sprintf "invalid argument: %A" req) ctx
        }

[<DataContract>]
type DeleteRequest =
    {
        [<field: DataMember(Name = "command")>]
        RawCommand : string
    }

let handleDeleteRequest (timer: ITimer): WebPart =
    fun (ctx : HttpContext) ->
        async {
            let req = ctx.request.rawForm |> fromJson<DeleteRequest>
            match req.RawCommand |> tryParseRawCommand with
            | Some command ->
                do! timer.DeleteTimer command
                return! OK (sprintf "command %s deleted" req.RawCommand) ctx
            | None -> return! NOT_FOUND (sprintf "invalid argument: %A" req) ctx
        }

let app timer =
    POST >=>
        choose [
            path "/api/create" >=> handleCreateRequest timer
            path "/api/delete" >=> handleDeleteRequest timer
        ]

[<EntryPoint>]
let main args =
    if args.Length <> 2 then failwith "Invalid arguments"
    use inMemoryTimer = createInMemoryTimer (float args.[0]) (postCommandHandler args.[1])
    printfn "InMemoryTimer is running... ticking every %A milliseconds" (float args.[0])
    startWebServer defaultConfig (app inMemoryTimer)
    0
