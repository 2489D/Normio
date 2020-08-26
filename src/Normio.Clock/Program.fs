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

// type Notifier (Id: uint32) =
//     do printfn "New notifier spawned! %u" Id

//     let doStartExam (req: RegisterReq) (stream: IServerStreamWriter<RegisterRes>) =
//         RegisterRes(RoomId = req.RoomId, ClockMessage = ClockMessage.StartExam)
//         |> stream.WriteAsync
//         |> Async.AwaitTask
//         |> Async.RunSynchronously
//         printfn "[*] The message is successfully sent..."

//     let doEndExam (req: RegisterReq) (stream: IServerStreamWriter<RegisterRes>) =
//         RegisterRes(RoomId = req.RoomId, ClockMessage = ClockMessage.EndExam)
//         |> stream.WriteAsync
//         |> Async.AwaitTask
//         |> Async.RunSynchronously
//         printfn "[*] The message is successfully sent..."

//     let agent = MailboxProcessor.Start (fun (inbox: MailboxProcessor<RegisterReq * IServerStreamWriter<RegisterRes>>) ->
//         let rec loop () = async {
//                 let! (req, stream) = inbox.Receive()
                
//                 req.ExamStart.ToDateTimeOffset() |> waitUntil
//                 printfn "[*] Sending start message..."
//                 doStartExam req stream

//                 req.ExamEnd.ToDateTimeOffset() |> waitUntil 
//                 printfn "[*] Sending end message..."
//                 doEndExam req stream

//                 return! loop ()
//             }

//         loop ()
//     )

//     member __.Work (req, stream) = agent.Post (req, stream)



// type Spawner () =
//     let spawnNotifier req stream i =
//         let notifier = Notifier(i)
//         notifier.Work (req, stream)

//     let agent = MailboxProcessor.Start (fun inbox -> 
//         let mutable i = 0u
//         let rec loop () = async {
//             // waits for a notification to arrive
//             let! (req, stream) = inbox.Receive () 
//             spawnNotifier req stream i
//             i <- i + 1u
//             return! loop ()
//         }

//         loop ()
//     )

//     member __.Spawn req stream =
//         printfn "Spawning new notifier %A" req 
//         agent.Post (req, stream)

type ClockServiceImpl() =
    inherit ClockService.ClockServiceBase()

    override __.Subscribe(req: RegisterReq, responseStream: IServerStreamWriter<RegisterRes>, ctx: ServerCallContext): Task =
        fun () ->
            // spawner.Spawn req responseStream
            req.ExamStart.ToDateTimeOffset() |> waitUntil // TODO
            RegisterRes(RoomId = req.RoomId, ClockMessage = ClockMessage.StartExam)
            |> responseStream.WriteAsync
            |> Async.AwaitTask
            |> Async.RunSynchronously

            req.ExamEnd.ToDateTimeOffset() |> waitUntil // TODO
            RegisterRes(RoomId = req.RoomId, ClockMessage = ClockMessage.EndExam)
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
