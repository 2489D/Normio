module Normio.Persistence.Projections

open System
open Normio.Core

type IExamAction =
    abstract OpenExam: examId:Guid -> title:ExamTitle40 -> Async<unit>
    abstract StartExam: examId:Guid -> Async<unit>
    abstract EndExam: examId:Guid -> Async<unit>
    abstract CloseExam: examId:Guid -> Async<unit>
    abstract AddStudent: examId:Guid -> Student -> Async<unit>
    abstract RemoveStudent: examId:Guid -> studentId:Guid -> Async<unit>
    abstract AddHost: examId:Guid -> host:Host -> Async<unit>
    abstract RemoveHost: examId:Guid -> hostId:Guid -> Async<unit>
    abstract CreateSubmission: examId:Guid -> submission:Submission -> Async<unit>
    abstract CreateQuestion: examId:Guid -> questionId:Guid -> Async<unit>
    abstract DeleteQuestion: examId:Guid -> questionId:Guid -> Async<unit>
    abstract ChangeTitle: examId:Guid -> title:ExamTitle40 -> Async<unit>


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
| QuestionCreated (examId, questionId) ->
    [actions.Exam.CreateQuestion examId questionId] |> Async.Parallel
| QuestionDeleted (examId, questionId) ->
    [actions.Exam.DeleteQuestion examId questionId] |> Async.Parallel
| TitleChanged (examId, title) ->
    [actions.Exam.ChangeTitle examId title] |> Async.Parallel
