namespace Normio.Persistence.ReadModels

open System
open System.Text.Json.Serialization
open FSharp.CosmosDb

open Normio.Core
open Normio.Core.Commands

[<JsonFSharpConverter(unionEncoding = JsonUnionEncoding.Untagged)>]
type ExamStatus =
    | BeforeExam
    | DuringExam
    | AfterExam

type TimerReadModel =
    { Command: Command
      Time: DateTime }

// missing created date time (on purpose?)
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
    [<JsonPropertyName("messages")>]
    Messages: Message seq
    [<JsonPropertyName("students")>]
    Students: Student seq
    [<JsonPropertyName("hosts")>]
    Hosts: Host seq
    [<JsonPropertyName("startDateTime")>]
    StartDateTime: DateTime
    [<JsonPropertyName("durationMins")>]
    DurationMinutes: float
} with
    static member Initial examId title startTime durationMinutes =
        { Id = examId
          ExamId = examId
          Status = BeforeExam
          Title = title
          Questions = Seq.empty
          Submissions = Seq.empty
          Messages = Seq.empty
          Students = Seq.empty
          Hosts = Seq.empty
          StartDateTime = startTime
          DurationMinutes = durationMinutes }
