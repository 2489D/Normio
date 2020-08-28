open System
open System.Threading
open System.Threading.Tasks
open Grpc.Core
open Google.Protobuf.WellKnownTypes
open Normio.Domain
open Normio.Protocol

type NormioServiceImpl() =
    inherit NormioService.NormioServiceBase()

    static let mutable roomPool: Room list = []

    override __.CreateRoom(req: CreateRoomReq, ctx: ServerCallContext): Task<CreateRoomRes> =
        printfn "[*] CreateRoom request received!"
        printfn "%A" req

        // TODO
        // Create Room on the Server

        let title = match RoomTitle40.create req.Title with
        | Some s -> s
        | None -> RoomTitle40 "Untitled"

        let newRoom = {
            Id = Guid.NewGuid()
            Title = title
        }
        roomPool <- newRoom :: roomPool

        printfn "[*] Current Room Pool: %A" roomPool

        let createdData = RoomData(RoomId = string newRoom.Id, Title = RoomTitle40.toString newRoom.Title)
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
