namespace Normio.Storage.ReadModels

open System
open Normio.Core.Domain

type ExamStatus =
    | BeforeExam
    | DuringExam
    | AfterExam

// TODO: student / host
type ExamReadModel = {
    Id: Guid
    Status: ExamStatus
    Title: ExamTitle40
    Questions: File list
    Submissions: Submission list
    Hosts: Map<Guid, Host>
    Students: Map<Guid, Student>
}
