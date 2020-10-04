module Normio.Core.States

open System
open Normio.Core.Domain
open Normio.Core.Events

/// 1. ExamIsClose None
/// 2. ExamIsWaiting Exam
/// 3. ExamIsRunning Exam
/// 4. ExamIsFinished Exam
/// 5. ExamIsClose Guid(Exam.Id)
type State =
    | ExamIsClose of Guid option
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
            Students = Map.empty
            Hosts = Map.empty
        }
    | ExamIsWaiting exam, ExamStarted _ ->
        ExamIsRunning exam
    | ExamIsWaiting exam, StudentEntered (_, student) ->
        { exam with Students = Map.add student.Id student exam.Students }
        |> ExamIsWaiting
    | ExamIsWaiting exam, StudentLeft (_, student) ->
        { exam with Students = Map.remove student.Id exam.Students }
        |> ExamIsWaiting
    | ExamIsWaiting exam, HostEntered (_, host) ->
        { exam with Hosts = Map.add host.Id host exam.Hosts }
        |> ExamIsWaiting
    | ExamIsWaiting exam, HostLeft (_, host) ->
        { exam with Hosts = Map.remove host.Id exam.Hosts }
        |> ExamIsWaiting
    | ExamIsWaiting exam, QuestionCreated (_, file) ->
        { exam with Questions = file :: exam.Questions  }
        |> ExamIsWaiting
    | ExamIsWaiting exam, QuestionDeleted (_, file) ->
        { exam with Questions = exam.Questions |> List.filter (fun f -> f.Id <> file.Id )}
        |> ExamIsWaiting
    | ExamIsWaiting exam, TitleChanged (_, newTitle) ->
        { exam with Title = newTitle }
        |> ExamIsWaiting
    | ExamIsRunning exam, ExamEnded _ ->
        ExamIsFinished exam
    | ExamIsFinished exam, ExamClosed _ ->
        ExamIsClose (Some exam.Id)
    | _ -> state

