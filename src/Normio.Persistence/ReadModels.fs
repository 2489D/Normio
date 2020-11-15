namespace Normio.Persistence.ReadModels

open System
open System.Text.Json.Serialization
open FSharp.CosmosDb

open Normio.Core

[<JsonFSharpConverter(unionEncoding = JsonUnionEncoding.ExternalTag)>]
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
    [<JsonPropertyName("students")>]
    Students: Student seq
}
