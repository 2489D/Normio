module Normio.Core.States

open System
open Normio.Core

/// State flow
/// 1. ExamIsClose None
/// 2. ExamIsWaiting Exam
/// 3. ExamIsRunning Exam
/// 4. ExamIsFinished Exam
/// 5. ExamIsClose Guid(Exam.Id)
type State =
    | ExamIsClose of examId:Guid option
    | ExamIsWaiting of Exam
    | ExamIsRunning of Exam
    | ExamIsFinished of Exam


(**
Style Guide:
1. Group (state, event) values by the state value. And separate them with empty lines.
2. The last (state, event) value grouped by (1) should be the one changing the state

3. Left to be improved:
How to make this `apply` function type-safe?
i.e.
All the (state, event) values here should be represented in the F# type system
so that whenever we miss to add or delete a (state, event) value,
the compiler can help us by issuing compilation error.

Now, this function is breaking the
'Make the illegal states unrepresentable' Principle.
**)
let apply state event =
    match state, event with
    | ExamIsClose None, ExamOpened (id, title) ->
        ExamIsWaiting (Exam.Initial id title)

    | ExamIsWaiting exam, StudentEntered (_, student) ->
        { exam with Students = Map.add student.Id student exam.Students }
        |> ExamIsWaiting
    | ExamIsWaiting exam, StudentLeft (_, studentId) ->
        { exam with Students = Map.remove studentId exam.Students }
        |> ExamIsWaiting
    | ExamIsWaiting exam, HostEntered (_, host) ->
        { exam with Hosts = Map.add host.Id host exam.Hosts }
        |> ExamIsWaiting
    | ExamIsWaiting exam, HostLeft (_, hostId) ->
        { exam with Hosts = Map.remove hostId exam.Hosts }
        |> ExamIsWaiting
    | ExamIsWaiting exam, QuestionCreated (_, question) ->
        { exam with Questions = question :: exam.Questions  }
        |> ExamIsWaiting
    | ExamIsWaiting exam, QuestionDeleted (_, questionId) ->
        { exam with Questions = exam.Questions |> List.filter (fun question -> question.Id <> questionId )}
        |> ExamIsWaiting
    | ExamIsWaiting exam, MessageSent (_, message) ->
        { exam with Messages = message :: exam.Messages }
        |> ExamIsWaiting
    | ExamIsWaiting exam, TitleChanged (_, newTitle) ->
        { exam with Title = newTitle }
        |> ExamIsWaiting
    | ExamIsWaiting exam, ExamStarted _ ->
        ExamIsRunning exam

    | ExamIsRunning exam, HostEntered (_, host) ->
        { exam with Hosts = Map.add host.Id host exam.Hosts }
        |> ExamIsRunning
    | ExamIsRunning exam, HostLeft (_, hostId) ->
        { exam with Hosts = Map.remove hostId exam.Hosts }
        |> ExamIsRunning
    | ExamIsRunning exam, SubmissionCreated (_, submission) ->
        { exam with Submissions = submission :: exam.Submissions }
        |> ExamIsRunning
    | ExamIsRunning exam, MessageSent (_, message) ->
        { exam with Messages = message :: exam.Messages }
        |> ExamIsRunning
    | ExamIsRunning exam, ExamEnded _ ->
        ExamIsFinished exam

    | ExamIsFinished exam, StudentLeft (_, studentId) ->
        { exam with Students = Map.remove studentId exam.Students }
        |> ExamIsFinished
    | ExamIsFinished exam, HostEntered (_, host) ->
        { exam with Hosts = Map.add host.Id host exam.Hosts }
        |> ExamIsFinished
    | ExamIsFinished exam, HostLeft (_, hostId) ->
        { exam with Hosts = Map.remove hostId exam.Hosts }
        |> ExamIsFinished
    | ExamIsFinished exam, MessageSent (_, message) ->
        { exam with Messages = message :: exam.Messages }
        |> ExamIsFinished
    | ExamIsFinished exam, ExamClosed _ ->
        ExamIsClose (Some exam.Id)

