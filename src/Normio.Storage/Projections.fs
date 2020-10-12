module Normio.Storage.Projections

open System
open Normio.Core.Domain
open Normio.Core.Events

(*
We are going to use `Event` returned from `evolve` function to populate ReadModels
e.g. ExamOpened (examId, title) will project the data to ExamForStudent
*)

type ExamActions = {
    OpenExam: Guid -> ExamTitle40 -> Async<unit>
    StartExam: Guid -> Async<unit>
    EndExam: Guid -> Async<unit>
    CloseExam: Guid -> Async<unit>
    AddStudent: Guid -> Student -> Async<unit>
    RemoveStudent: Guid -> Guid -> Async<unit>
    AddHost: Guid -> Host -> Async<unit>
    RemoveHost: Guid -> Guid -> Async<unit>
    CreateSubmission: Guid -> Submission -> Async<unit>
    CreateQuestion: Guid -> File -> Async<unit>
    DeleteQuestion: Guid -> File -> Async<unit>
    ChangeTitle: Guid -> ExamTitle40 -> Async<unit>
}

type ProjectionActions = {
    Exam: ExamActions
}

let projectReadModel actions = function
| ExamOpened (examId, title) ->
    [actions.Exam.OpenExam examId title] |> Async.Parallel
| ExamStarted examId ->
    [actions.Exam.StartExam examId] |> Async.Parallel
| ExamEnded examId ->
    [actions.Exam.EndExam examId] |> Async.Parallel
| ExamClosed examId ->
    [actions.Exam.CloseExam examId] |> Async.Parallel
| StudentEntered (examId, student) ->
    [actions.Exam.AddStudent examId student] |> Async.Parallel
| StudentLeft (examId, studentId) ->
    [actions.Exam.RemoveStudent examId studentId] |> Async.Parallel
| HostEntered (examId, host) ->
    [actions.Exam.AddHost examId host] |> Async.Parallel
| HostLeft (examId, hostId) ->
    [actions.Exam.RemoveHost examId hostId] |> Async.Parallel
| SubmissionCreated (examId, submission) ->
    [actions.Exam.CreateSubmission examId submission] |> Async.Parallel
| QuestionCreated (examId, file) ->
    [actions.Exam.CreateQuestion examId file] |> Async.Parallel
| QuestionDeleted (examId, file) ->
    [actions.Exam.DeleteQuestion examId file] |> Async.Parallel
| TitleChanged (examId, title) ->
    [actions.Exam.ChangeTitle examId title] |> Async.Parallel
