[<AutoOpen>]
module Normio.Persistence.Queries

open System
open Normio.Persistence.ReadModels

type TimerQueries = {
    GetCommandsById: Guid -> Async<TimerReadModel list>
}

type ExamQueries = {
    GetExamByExamId: Guid -> Async<ExamReadModel option>
}

type Queries = {
    Exam: ExamQueries
    Timer: TimerQueries
}
