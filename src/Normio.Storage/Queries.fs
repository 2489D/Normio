module Normio.Queries

open System
open Normio.Domain
open Normio.ReadModels

type ExamQueries = {
    GetExam: Guid -> Async<ExamReadModel>
}

type Queries = {
    Exam: ExamQueries
}