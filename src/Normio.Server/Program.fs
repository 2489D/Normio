open System
open System.Threading
open System.Threading.Tasks
open Grpc.Core
open Google.Protobuf.WellKnownTypes
open Normio.Domain
open Normio.Protocol

type UserClientRequest =
    | CreateRoom of RoomTitle40 * AsyncReplyChannel<RoomId>
    | ExpireRoom of RoomId

type RoomWrapper() =
    let mutable roomId = Guid.NewGuid()
    let mutable running = false
    let roomAgent = MailboxProcessor.Start(fun inbox ->
        let rec processState oldState = async {
            let newState = //TODO
            return! loop newState
        }
        loop initialState
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

type RoomPool() =
    let [<Literal>] POOL_SIZE = 100
    let roomPool = List.init 100 (fun _ -> RoomWrapper())
    let roomPoolAgent = MailboxProcessor.Start(fun inbox ->
        let rec loop () = async {
            let! msg = inbox.Receive()
            let result = dealWithMessage msg
            match result with
            | Ok -> ()
            | Error e ->
                // TODO: Recover errors
                ()
            return! loop ()
        }
            
        loop ()
    )

    let dealWithMessage msg: Result<unit, ServerError> =
        match msg with
        | CreateRoom reply -> // Acquire a room's id
            match findFreeRoom with
            | Some room ->
                reply.Reply(room.Id)
                Ok ()
            | None ->
                Error PoolIsFull
        | ExpireRoom roomId ->
            match searchById roomId with
            | Some room ->
                room.Release()
                Ok ()
            | _ ->
                Error NoSuchId

    let searchById roomId = roomPool |> List.tryFind (fun room -> room.Id = roomId)
    let findFreeRoom = roomPool |> List.tryFind (fun room -> room.IsAvailable)

    member __.CreateRoom = roomPool.PostAndReply (fun reply -> CreateRoom reply)
    member __.ExpireRoom roomId = roomPool.Post roomId

// Message Processing Agent
type NormioServiceImpl() =
    inherit NormioService.NormioServiceBase()

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


    //TODO
    // Data Cleansing
    // Natives types -> Wrapped types
    override __.CreateRoom(req: CreateRoomReq, ctx: ServerCallContext): Task<CreateRoomRes> =
        printfn "[*] Request received!"
        printfn "%A" (req, CreateRoom)

        // TODO: Create Room on the Server
        roomPoolAgent.Post (req, CreateRoom)

        let createdData = RoomData(RoomId = "we dont know", Title = req.Title)
        CreateRoomRes(RoomData = createdData, CreationStatus="OK")
        |> Task.FromResult

let [<Literal>] Address = "127.0.0.1"
let [<Literal>] Port = 8081

[<EntryPoint>]
let main argv =
    let server = Server()
    let service = NormioServiceImpl()
    server.Services.Add(NormioService.BindService(service))
    server.Ports.Add(ServerPort(Address, Port, ServerCredentials.Insecure)) |> ignore
    server.Start()

    printfn "##### Normio Server #####"
    printfn "launched..."

    while true do ()

    0 // return an integer exit code
