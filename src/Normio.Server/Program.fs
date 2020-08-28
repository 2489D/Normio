open System
open System.Threading
open System.Threading.Tasks
open Grpc.Core
open Google.Protobuf.WellKnownTypes
open Normio.Protocol

type NormioServiceImpl() =
    inherit NormioService.NormioServiceBase()

    override __.CreateRoom(req: CreateRoomReq, ctx: ServerCallContext): Task<CreateRoomRes> =
        printfn "[*] CreateRoom request received!"
        printfn "%A" req
        let title = req.Title
        let createdData = RoomData(RoomId = string (Guid.NewGuid()), Title = title)
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
