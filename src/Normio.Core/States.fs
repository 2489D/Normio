module Normio.Core.States

open System
open Normio.Core

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

let apply state event =
    match state, event with
    | ExamIsClose None, ExamOpened (id, title) ->
        ExamIsWaiting {
            Id = id
            Title = title
            Questions = []
            Submissions = []
            Students = Map.empty
            Hosts = Map.empty
        }
    | ExamIsWaiting exam, ExamStarted _ ->
        ExamIsRunning exam
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
    | ExamIsWaiting exam, QuestionCreated (_, questionId) ->
        { exam with Questions = questionId :: exam.Questions  }
        |> ExamIsWaiting
    | ExamIsWaiting exam, QuestionDeleted (_, questionId) ->
        { exam with Questions = exam.Questions |> List.filter (fun questionId' -> questionId' <> questionId )}
        |> ExamIsWaiting
    | ExamIsWaiting exam, TitleChanged (_, newTitle) ->
        { exam with Title = newTitle }
        |> ExamIsWaiting
    | ExamIsRunning exam, SubmissionCreated (_, submission) ->
        { exam with Submissions = submission :: exam.Submissions }
        |> ExamIsRunning
    | ExamIsRunning exam, ExamEnded _ ->
        ExamIsFinished exam
    | ExamIsFinished exam, ExamClosed _ ->
        ExamIsClose (Some exam.Id)
    | _ -> state

