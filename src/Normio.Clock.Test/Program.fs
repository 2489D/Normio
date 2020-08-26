// Learn more about F# at http://fsharp.org

open System
open Grpc.Core
open Google.Protobuf.WellKnownTypes
open Normio.Clock

type ClockServiceImpl(client: ClockService.ClockServiceClient) =
    member __.Subscribe(req: RegisterReq) =
        let subs = client.Subscribe(req)
        let stream = subs.ResponseStream
        let mutable i = 0
        while stream.MoveNext(Threading.CancellationToken.None) |> Async.AwaitTask |> Async.RunSynchronously do
            let curr = stream.Current
            (curr.RoomId, curr.ClockMessage)
            |> printfn "Got a response %d %A" i
            i <- i + 1
        

[<EntryPoint>]
let main argv =
    let channel = Channel("127.0.0.1:8080", ChannelCredentials.Insecure)
    let client = ClockServiceImpl(ClockService.ClockServiceClient(channel))

    let args = argv |> List.ofSeq
    let duration =
        match args with
        | "--duration" :: [seconds] -> float seconds
        | _ -> 0.5
    
    let now = DateTimeOffset.Now
    let req () = RegisterReq(
                    RoomId = Guid.NewGuid().ToString(),
                    ExamStart = Timestamp.FromDateTimeOffset now,
                    ExamEnd = Timestamp.FromDateTimeOffset (now.AddSeconds(duration))
                )
    
    for i in 1..10000 do
        client.Subscribe(req())
        
    channel.ShutdownAsync().Wait()
    
    0 // return an integer exit code
