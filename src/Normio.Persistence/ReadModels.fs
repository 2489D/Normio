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
    ExamId: Guid
    Status: ExamStatus
    Title: ExamTitle40
    Questions: Question seq
    Submissions: Submission seq
    Hosts: Host seq
    Students: Student seq
}
