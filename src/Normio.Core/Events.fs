namespace Normio.Core

open System
open System.Text.Json.Serialization
open Normio.Core

[<AutoOpen>]
module Events =
    [<JsonFSharpConverter(unionEncoding = (JsonUnionEncoding.ExternalTag ||| JsonUnionEncoding.NamedFields))>]
    type Event =
        | ExamOpened of examId:Guid * title:ExamTitle40
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
