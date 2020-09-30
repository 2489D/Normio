module Normio.ReadModels

open System
open Normio.Domain

(*
    Suppose all users need full date of the exam
    for simplicity
*)

type ExamStatus =
    | BeforeExam
    | DuringExam
    | AfterExam

type ExamReadModel = {
    Id: Guid
    Status: ExamStatus
    Title: string
    Hosts: Map<Guid, Host>
    Students: Map<Guid, Student>
    Questions: File list
}

