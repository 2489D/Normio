namespace Normio.Core

open System
open System.Text.Json.Serialization
open Normio.Core

[<AutoOpen>]
module Events =
    [<JsonFSharpConverter(unionEncoding = (JsonUnionEncoding.ExternalTag ||| JsonUnionEncoding.NamedFields))>]
    type Event =
        | ExamOpened of examId:Guid * title:ExamTitle40 * startTime:DateTime * duration: TimeSpan
        | ExamStarted of examId:Guid
        | ExamEnded of examId:Guid
        | ExamClosed of examId:Guid

        | StudentEntered of examId:Guid * student:Student
        | StudentLeft of examId:Guid * studentId:Guid
        | HostEntered of examId:Guid * host:Host
        | HostLeft of examId:Guid * hostId:Guid

        | SubmissionCreated of examId:Guid * submission:Submission
        | QuestionCreated of examId:Guid * question:Question
        | QuestionDeleted of examId:Guid * questionId:Guid

        | MessageSent of examId: Guid * message:Message

        | TitleChanged of examId:Guid * title:ExamTitle40
        
        with
            member this.ExamId =
                match this with
                | ExamOpened (id, _, _, _) -> id
                | ExamStarted id -> id
                | ExamEnded id -> id
                | ExamClosed id -> id
                | StudentEntered (id, _) -> id
                | StudentLeft (id, _) -> id
                | HostEntered (id, _) -> id
                | HostLeft (id, _) -> id
                | SubmissionCreated (id, _) -> id
                | QuestionCreated (id, _) -> id
                | QuestionDeleted (id, _) -> id
                | MessageSent (id, _) -> id
                | TitleChanged (id, _) -> id
