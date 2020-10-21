module Normio.Persistence.Queries

open System
open Normio.Persistence.ReadModels

type ExamQueries = {
    GetExamByExamId: Guid -> Async<ExamReadModel option>
}

type Queries = {
    Exam: ExamQueries
}
