module Normio.Persistence.Projections

open System
open Normio.Core

type IExamAction =
    abstract OpenExam: Guid -> ExamTitle40 -> Async<unit>
    abstract StartExam: Guid -> Async<unit>
    abstract EndExam: Guid -> Async<unit>
    abstract CloseExam: Guid -> Async<unit>
    abstract AddStudent: Guid -> Student -> Async<unit>
    abstract RemoveStudent: Guid -> Guid -> Async<unit>
    abstract AddHost: Guid -> Host -> Async<unit>
    abstract RemoveHost: Guid -> Guid -> Async<unit>
    abstract CreateSubmission: Guid -> Submission -> Async<unit>
    abstract CreateQuestion: Guid -> File -> Async<unit>
    abstract DeleteQuestion: Guid -> Guid -> Async<unit>
    abstract ChangeTitle: Guid -> ExamTitle40 -> Async<unit>


type ProjectionActions = {
    Exam: IExamAction
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
| QuestionDeleted (examId, fileId) ->
    [actions.Exam.DeleteQuestion examId fileId] |> Async.Parallel
| TitleChanged (examId, title) ->
    [actions.Exam.ChangeTitle examId title] |> Async.Parallel
