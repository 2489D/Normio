module Normio.Storage.ReadModels

open System
open Normio.Core.Domain

(*
    Suppose all users need full data of the exam
    for simplicity
*)

type ExamStatus =
    | BeforeExam
    | DuringExam
    | AfterExam

type ExamReadModel = {
    Id: Guid
    Status: ExamStatus
    Title: ExamTitle40
    Hosts: Map<Guid, Host>
    Students: Map<Guid, Student>
    Questions: File list
}
