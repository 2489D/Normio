module Normio.Queries

open System
open Normio.Domain

type ExamQueries = {
    GetExam: Guid -> Async<Exam>
}

type Queries = {
    Exam: ExamQueries
}