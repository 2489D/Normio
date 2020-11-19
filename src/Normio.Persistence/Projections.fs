module Normio.Persistence.Projections

open System
open Normio.Core
open Normio.Core.Commands

type ITimerAction =
    abstract CreateTimer: command:Command -> time:DateTime -> Async<unit>
    abstract RemoveTimer: command:Command -> Async<unit>

type IExamAction =
    abstract OpenExam: examId:Guid -> title:ExamTitle40 -> startTime:DateTime -> duration:TimeSpan -> Async<unit>
    abstract StartExam: examId:Guid -> Async<unit>
    abstract EndExam: examId:Guid -> Async<unit>
    abstract CloseExam: examId:Guid -> Async<unit>
    abstract LetStudentIn: examId:Guid -> student:Student -> Async<unit>
    abstract LetStudentOut: examId:Guid -> studentId:Guid -> Async<unit>
    abstract LetHostIn: examId:Guid -> host:Host -> Async<unit>
    abstract LetHostOut: examId:Guid -> hostId:Guid -> Async<unit>
    abstract CreateSubmission: examId:Guid -> submission:Submission -> Async<unit>
    abstract CreateQuestion: examId:Guid -> question:Question -> Async<unit>
    abstract DeleteQuestion: examId:Guid -> questionId:Guid -> Async<unit>
    abstract SendMessage: examId:Guid -> message:Message -> Async<unit>
    abstract ChangeTitle: examId:Guid -> title:ExamTitle40 -> Async<unit>


type ProjectionActions = {
    Exam: IExamAction
    Timer: ITimerAction
}

let projectReadModel actions = function
| ExamOpened (examId, title, startTime, duration) ->
    [actions.Exam.OpenExam examId title startTime duration
     actions.Timer.CreateTimer (StartExam(examId)) startTime
     actions.Timer.CreateTimer (EndExam(examId)) (startTime + duration)] |> Async.Parallel
| ExamStarted examId ->
    [actions.Exam.StartExam examId
     actions.Timer.RemoveTimer <| StartExam(examId)] |> Async.Parallel
| ExamEnded examId ->
    [actions.Exam.EndExam examId
     actions.Timer.RemoveTimer <| EndExam(examId) ] |> Async.Parallel
| ExamClosed examId ->
    [actions.Exam.CloseExam examId] |> Async.Parallel
| StudentAdded _
| StudentRemoved _ ->
    [] |> Async.Parallel
| StudentEntered (examId, student) ->
    [actions.Exam.LetStudentIn examId student] |> Async.Parallel
| StudentLeft (examId, studentId) ->
    [actions.Exam.LetStudentOut examId studentId] |> Async.Parallel
| HostAdded _
| HostRemoved _ ->
    [] |> Async.Parallel
| HostEntered (examId, host) ->
    [actions.Exam.LetHostIn examId host] |> Async.Parallel
| HostLeft (examId, hostId) ->
    [actions.Exam.LetHostOut examId hostId] |> Async.Parallel
| SubmissionCreated (examId, submission) ->
    [actions.Exam.CreateSubmission examId submission] |> Async.Parallel
| QuestionCreated (examId, question) ->
    [actions.Exam.CreateQuestion examId question] |> Async.Parallel
| QuestionDeleted (examId, questionId) ->
    [actions.Exam.DeleteQuestion examId questionId] |> Async.Parallel
| MessageSent (examId, message) ->
    [actions.Exam.SendMessage examId message] |> Async.Parallel
| TitleChanged (examId, title) ->
    [actions.Exam.ChangeTitle examId title] |> Async.Parallel
