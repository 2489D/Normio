namespace Normio.Persistence.ReadModels

open System
open System.Text.Json.Serialization
open FSharp.CosmosDb

open Normio.Core

[<JsonFSharpConverter(unionEncoding = JsonUnionEncoding.Untagged)>]
type ExamStatus =
    | BeforeExam
    | DuringExam
    | AfterExam

[<JsonFSharpConverter>]
type ExamReadModel = {
    [<JsonPropertyName("id")>]
    Id: Guid
    [<PartitionKey>]
    [<JsonPropertyName("examId")>]
    ExamId: Guid
    [<JsonPropertyName("status")>]
    Status: ExamStatus
    [<JsonPropertyName("title")>]
    Title: ExamTitle40
    [<JsonPropertyName("questions")>]
    Questions: Question seq
    [<JsonPropertyName("submission")>]
    Submissions: Submission seq
    [<JsonPropertyName("hosts")>]
    Hosts: Host seq
    [<JsonPropertyName("messages")>]
    Messages: Message seq
    [<JsonPropertyName("students")>]
    Students: Student seq
    [<JsonPropertyName("startDateTime")>]
    StartDateTime: DateTime option
    [<JsonPropertyName("duration")>]
    Duration: TimeSpan option
} with
    static member Initial examId title =
        { Id = examId
          ExamId = examId
          Status = BeforeExam
          Title = title
          Students = Seq.empty
          Hosts = Seq.empty
          Questions = Seq.empty
          Messages = Seq.empty
          Submissions = Seq.empty
          StartDateTime = None
          Duration = None }
