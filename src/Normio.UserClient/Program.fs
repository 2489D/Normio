open System
open Grpc.Core
open Normio.Protocol

type NormioServiceImpl(client: NormioService.NormioServiceClient) =
    member __.CreateRoom(req: CreateRoomReq) =
        client.CreateRoom(req)
        |> printfn "%A"


[<EntryPoint>]
let main argv =
    printfn "##### Normio User Client (Test) #####"
    let channel = Channel("127.0.0.1:8081", ChannelCredentials.Insecure)
    let client = NormioServiceImpl(NormioService.NormioServiceClient(channel))

    let req = CreateRoomReq(Title = "hello")

    client.CreateRoom(req)

    channel.ShutdownAsync().Wait()

    0 // return an integer exit code
