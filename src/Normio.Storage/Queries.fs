module Normio.Storage.Queries

open System
open Normio.Storage.ReadModels

type ExamQueries = {
    GetExamByExamId: Guid -> Async<ExamReadModel option>
}

type Queries = {
    Exam: ExamQueries
}
