namespace Normio.Persistence.ReadModels

open System
open System.Text.Json.Serialization
open Normio.Core.Domain

type ExamStatus =
    | BeforeExam
    | DuringExam
    | AfterExam

[<JsonFSharpConverter>]
type ExamReadModel = {
    Id: Guid
    ExamId: Guid
    Status: ExamStatus
    Title: ExamTitle40
    Questions: File list
    Submissions: Submission list
    Hosts: Map<Guid, Host>
    Students: Map<Guid, Student>
}
