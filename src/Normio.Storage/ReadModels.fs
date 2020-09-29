module Normio.ReadModels

open System
open Normio.Domain

type ExamStatus =
    | BeforeExam of Guid
    | DuringExam of Guid
    | AfterExam of Guid

type ExamReadModel = {
    Id: Guid
    Status: ExamStatus
    Title: string
    Hosts: Host list
    Students: Student list
    Questions: File list
}

