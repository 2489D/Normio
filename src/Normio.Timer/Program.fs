module Normio.Timer.App

open System

open System.Runtime.Serialization
open FsHttp
open Suave
open Suave.Json
open Suave.Filters
open Suave.Operators
open Suave.Successful

let postStartExamCommand (examId: Guid) =
    async {
        let backendUrl = "https://localhost:5001/"
        let reqBody = sprintf """{ "examId" : %A }""" examId
        let! res = httpAsync {
            POST (backendUrl + "startExam")
            body
            json reqBody
        }
        ()
    }
    
[<DataContract>]
type Reservation =
    {
        [<field: DataMember(Name = "when")>]
        When : DateTime
        [<field: DataMember(Name = "examId")>]
        ExamId : Guid
    }

let handleReservation (timer: ITimer): WebPart =
    fun (ctx : HttpContext) ->
        async {
            // FIXME: DateTime ser/de is fucking bad
            let reservation = ctx.request.rawForm |> fromJson<Reservation>
            printfn "%A" reservation
            let taskId = timer.SetTimer reservation.When (postStartExamCommand reservation.ExamId)
            return! CREATED (sprintf "exam %A reserved" reservation.ExamId) ctx
        }
    
let app timer =
    POST >=> choose [
        path "/reserve" >=> handleReservation timer
    ]

[<EntryPoint>]
let main argv =
    use inMemoryTimer = createInMemoryTimer (float argv.[0])
    printfn "InMemoryTimer is running... ticking every %A milliseconds" (float argv.[0])
    startWebServer defaultConfig (app inMemoryTimer)
    0
