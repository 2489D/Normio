open System
open System.Threading
open System.Threading.Tasks
open Grpc.Core
open Google.Protobuf.WellKnownTypes
open Normio.Domain
open Normio.States
open Normio.Protocol

/// Id is for the wrapper
/// RoomData is contained by the Room type (see Normio.Domain)
type RoomWrapper() =
    let mutable roomId = Guid.NewGuid()
    let mutable running = false

    let initialState = RoomIsWaiting {
        Title = RoomTitle40 "untitled"
    }
    let roomAgent = MailboxProcessor.Start(fun inbox ->
        let rec loop oldState = async {
            let newState = oldState
            return! loop newState
        }
        loop (initialState)
    )

    member __.Id = roomId
    member __.IsAvailable = not running
    member __.Acquire() = running <- true
    member __.Release() =
        // TODO: Do release jobs
        roomId <- Guid.NewGuid() // Security Reasons
        running <- false

type RoomPoolError =
    | PoolIsFull
    | NoSuchId

type UserClientRequest =
    | CreateRoom of RoomTitle40 * AsyncReplyChannel<Result<Guid, RoomPoolError>>
    | ExpireRoom of Guid * AsyncReplyChannel<Result<unit, RoomPoolError>>

type RoomPool() =
    let [<Literal>] POOL_SIZE = 10 
    let roomPool = List.init POOL_SIZE (fun _ -> RoomWrapper())

    let searchById roomId = roomPool |> List.tryFind (fun room -> room.Id = roomId)
    let findFreeRoom () = roomPool |> List.tryFind (fun room -> room.IsAvailable)
    let releaseById roomId =
        match searchById roomId with
        | Some room ->
            room.Release()
            Ok ()
        | None -> Error NoSuchId
    let acquireFreeRoom () =
        match findFreeRoom() with
        | Some room ->
            room.Acquire()
            Ok room
        | None ->
            Error PoolIsFull

    let dealWithMessage msg =
        match msg with
        | CreateRoom (title, reply) -> // Acquire a room's id
            match acquireFreeRoom() with
            | Ok room -> reply.Reply(Ok room.Id) // TODO: title?
            | Error e -> reply.Reply(Error e)
        | ExpireRoom (roomId, reply) ->
            match releaseById roomId with
            | Ok _ -> reply.Reply(Ok ())
            | Error e -> reply.Reply(Error e)

    let roomPoolAgent = MailboxProcessor.Start(fun inbox ->
        let rec loop () = async {
            let! msg = inbox.Receive()
            dealWithMessage msg |> ignore
            return! loop ()
        }
            
        loop ()
    )


    member __.CreateRoom title = roomPoolAgent.PostAndReply (fun reply -> CreateRoom (title, reply))
    member __.ExpireRoom roomId = roomPoolAgent.Post roomId

// Message Processing Agent
type NormioServiceImpl(roomPools: RoomPool list) =
    inherit NormioService.NormioServiceBase()

// replaced by upper codes
(*
    static let updatePool oldPool (data: CreateRoomReq) =
        let title = match RoomTitle40.create data.Title with
        | Some s -> s
        | None -> RoomTitle40 "Untitled"
        let newRoom = Room.init title
        newRoom :: oldPool

    static let roomPoolAgent = MailboxProcessor.Start(fun (inbox:MailboxProcessor<CreateRoomReq * ClientRuquest>) ->
        let rec processMsg oldPool = async {
            let! (data, reqKind) = inbox.Receive()
            let newPool = match reqKind with
                | _ -> updatePool oldPool data
            List.length newPool |> printfn "[*] Current Room Pool Size: %d" 
            printfn "[*] Current Room Pool"
            newPool |> List.iter (printfn "-- %A")
            return! processMsg newPool
        }

        processMsg []
    )
*)

    //TODO
    // Data Cleansing
    // Natives types -> Wrapped types
    override __.CreateRoom(req: CreateRoomReq, ctx: ServerCallContext): Task<CreateRoomRes> =
        printfn "[*] CreateRoom Request received!"

        // TODO: Call RoomPool
        let roomPool = roomPools |> List.head
        let responseRoomId = req.Title |> RoomTitle40.create
                            |> Option.fold (fun _ s -> s) (RoomTitle40 "")
                            |> roomPool.CreateRoom

        printfn "%A" (req, responseRoomId)

        let createdData = RoomData(RoomId = responseRoomId.ToString(), Title = req.Title)
        CreateRoomRes(RoomData = createdData, CreationStatus = "OK")
        |> Task.FromResult

// Server Configuration
let [<Literal>] Address = "127.0.0.1"
let [<Literal>] Port = 8081
// TODO: ServerCredentials setup

[<EntryPoint>]
let main argv =
    let server = Server()
    let roomPools = [RoomPool()]
    let service = NormioServiceImpl(roomPools)
    server.Services.Add(NormioService.BindService(service))
    server.Ports.Add(ServerPort(Address, Port, ServerCredentials.Insecure)) |> ignore
    server.Start()

    printfn "##### Normio Server #####"
    printfn "launched..."

    while true do ()

    0 // return an integer exit code
