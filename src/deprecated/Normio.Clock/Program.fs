open System
open System.IO
open System.Threading.Tasks
open Grpc.Core
open Normio.Clock
open Google.Protobuf

type NotiTime = System.DateTimeOffset

type NotiAction = Guid -> NotiTime -> Async<unit>

type Notification = {
    Id: Guid
    StartExam: NotiTime
    EndExam: NotiTime
}

// TODO: polling -> interrupt mechanism
let waitUntil time =
    let isOver () =
        DateTimeOffset.Compare(DateTimeOffset.Now, time) >= 0
    let rec loop () =
        if isOver ()
        then ()
        else loop ()
    loop ()

type ClockServiceImpl() =
    inherit ClockService.ClockServiceBase()

    override __.Subscribe(req: RegisterReq, responseStream: IServerStreamWriter<RegisterRes>, ctx: ServerCallContext): Task =
        fun () ->
            // spawner.Spawn req responseStream
            req.ExamStart.ToDateTimeOffset() |> waitUntil // TODO
            RegisterRes(ClockMessage = ClockMessage.StartExam)
            |> responseStream.WriteAsync
            |> Async.AwaitTask
            |> Async.RunSynchronously

            req.ExamEnd.ToDateTimeOffset() |> waitUntil // TODO
            RegisterRes(ClockMessage = ClockMessage.EndExam)
            |> responseStream.WriteAsync
            |> Async.AwaitTask
            |> Async.RunSynchronously
        |> Task.Run

let [<Literal>] Address = "127.0.0.1"
let [<Literal>] Port = 8080

[<EntryPoint>]
let main argv =
    printfn "##### Normio Clock Server #####"
    let server = Server()
    let service = ClockServiceImpl()
    server.Services.Add(ClockService.BindService(service))
    server.Ports.Add(ServerPort(Address, Port, ServerCredentials.Insecure)) |> ignore
    server.Start()

    while true do
        ()

    server.ShutdownAsync().Wait()
    0 // return an integer exit code
