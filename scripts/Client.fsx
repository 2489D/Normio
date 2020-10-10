// Run this script with `dotnet fsi --langversion:preview`
// the reference by nuget below needs the option

#r "nuget: Microsoft.AspNetCore.SignalR.Client"

open System
open Microsoft.AspNetCore.SignalR.Client

let conn =
    (HubConnectionBuilder())
        .WithUrl("https://localhost:5001/signal")
        .Build()

conn.On("ReceiveEvent", fun (event: string) -> printfn "%A" event)

try
    conn.StartAsync().Wait()
    printfn "Connection Established: %A" conn.ConnectionId
with
    | ex -> printfn "connection error: %A" ex

Console.ReadKey() |> ignore
