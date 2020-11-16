namespace Normio.Commands.Api

open System
open System.Text.Json.Serialization

open Normio.Commands.Api
open Normio.Core
open Normio.Core.Commands

[<AutoOpen>]
module SendMessage =
    type MessageKind =
        | FromStudentToHost = 0
        | FromHostToHosts = 1
        | FromHostToStudents = 2
        | Notice = 3

    [<CLIMutable>]
    type SendMessageRequest =
        {
            [<JsonPropertyName("examId")>]
            ExamId : Guid
            [<JsonPropertyName("messageKind")>]
            MessageKind : MessageKind
            [<JsonPropertyName("senderId")>]
            SenderId: Guid
            [<JsonPropertyName("receiverId")>]
            ReceiverId: Guid seq
            [<JsonPropertyName("content")>]
            Content: string
        }

    let validateSendMessage (req: SendMessageRequest) = async {
        match req.MessageKind with
        | MessageKind.FromStudentToHost
        | MessageKind.FromHostToHosts
        | MessageKind.FromHostToStudents
        | MessageKind.Notice ->
            match MessageContent.Create req.Content with
            | Ok content -> return Ok (req.ExamId, req.MessageKind, req.SenderId, req.ReceiverId, content)
            | Error err -> return Error <| err.ToString()
        | _ -> return Error "Unacceptable message kind"
    }

    let toSendMessageCommand (examId, messageKind, senderId, receiverId, content) =
        let message: Message = 
            match messageKind with
            | MessageKind.FromStudentToHost ->
                MessageFromStudentToHost
                <| { Id = Guid.NewGuid()
                     ExamId = examId
                     SenderStudent = senderId
                     ReceiverHost = receiverId |> Seq.head
                     CreatedDateTime = DateTime.Now
                     Content = content }
            | MessageKind.FromHostToHosts ->
                MessageFromHostToHosts
                <| { Id = Guid.NewGuid()
                     ExamId = examId
                     SenderHost = senderId
                     ReceiverHosts = receiverId |> List.ofSeq
                     CreatedDateTime = DateTime.Now
                     Content = content }
            | MessageKind.FromHostToStudents ->
               MessageFromHostToStudents
               <| { Id = Guid.NewGuid()
                    ExamId = examId
                    SenderHost = senderId
                    ReceiverStudents = receiverId |> List.ofSeq
                    CreatedDateTime = DateTime.Now
                    Content = content}
            | MessageKind.Notice ->
                Notice
                <| { Id = Guid.NewGuid()
                     ExamId = examId
                     SenderHost = senderId
                     CreatedDateTime = DateTime.Now
                     Content = content }
            | _ -> failwith "Invalid message kind" // this must not happen
        SendMessage (examId, message)

    let sendMessageCommander = {
        Validate = validateSendMessage
        ToCommand = toSendMessageCommand
    }

