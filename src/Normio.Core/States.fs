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
    | ExamIsClose of examId: Guid option
    | ExamIsWaiting of exam:Exam
    | ExamIsRunning of exam:Exam
    | ExamIsFinished of exam:Exam

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
    | ExamIsClose None, ExamOpened (id, title, startTime, duration) ->
        ExamIsWaiting(Exam.Initial id title startTime duration)

    | ExamIsWaiting exam, StudentAdded (_, student) ->
        { exam with
              Students =
                  exam.Students
                  |> Map.add student.Id (StudentInExam.Disconnected student) }
        |> ExamIsWaiting
    | ExamIsWaiting exam, StudentRemoved (_, studentId) ->
        { exam with
              Students = exam.Students |> Map.remove studentId }
        |> ExamIsWaiting
    | ExamIsWaiting exam, StudentEntered (_, student) ->
        match exam.Students |> Map.tryFind student.Id with
        | Some student ->
            { exam with
                  Students =
                      exam.Students
                      |> Map.add student.Id (StudentInExam.Connected student.Value) }
            |> ExamIsWaiting
        | _ -> ExamIsWaiting exam
    | ExamIsWaiting exam, StudentLeft (_, studentId) ->
        match exam.Students |> Map.tryFind studentId with
        | Some student ->
            { exam with
                  Students =
                      exam.Students
                      |> Map.add studentId (StudentInExam.Disconnected student.Value) }
            |> ExamIsWaiting
        | _ -> ExamIsWaiting exam
    | ExamIsWaiting exam, HostAdded (_, host) ->
        { exam with
              Hosts =
                  exam.Hosts
                  |> Map.add host.Id (HostInExam.Disconnected host) }
        |> ExamIsWaiting
    | ExamIsWaiting exam, HostRemoved (_, hostId) ->
        { exam with
              Hosts = exam.Hosts |> Map.remove hostId }
        |> ExamIsWaiting
    | ExamIsWaiting exam, HostEntered (_, host) ->
        match exam.Hosts |> Map.tryFind host.Id with
        | Some host ->
            { exam with
                  Hosts =
                      exam.Hosts
                      |> Map.add host.Id (HostInExam.Connected host.Value) }
            |> ExamIsWaiting
        | _ -> ExamIsWaiting exam
    | ExamIsWaiting exam, HostLeft (_, hostId) ->
        let host = exam.Hosts |> Map.tryFind hostId
        match host with
        | Some h ->
            { exam with
                  Hosts =
                      exam.Hosts
                      |> Map.add hostId (HostInExam.Disconnected h.Value) }
            |> ExamIsWaiting
        | _ -> ExamIsWaiting exam
    | ExamIsWaiting exam, QuestionCreated (_, question) ->
        { exam with
              Questions = question :: exam.Questions }
        |> ExamIsWaiting
    | ExamIsWaiting exam, QuestionDeleted (_, questionId) ->
        { exam with
              Questions =
                  exam.Questions
                  |> List.filter (fun question -> question.Id <> questionId) }
        |> ExamIsWaiting
    | ExamIsWaiting exam, MessageSent (_, message) ->
        { exam with
              Messages = message :: exam.Messages }
        |> ExamIsWaiting
    | ExamIsWaiting exam, TitleChanged (_, newTitle) -> { exam with Title = newTitle } |> ExamIsWaiting
    | ExamIsWaiting exam, ExamStarted _ -> ExamIsRunning exam

    | ExamIsRunning exam, HostEntered (_, host) ->
        let host = exam.Hosts |> Map.tryFind host.Id
        match host with
        | Some host ->
            { exam with
                  Hosts =
                      exam.Hosts
                      |> Map.add host.Id (HostInExam.Connected host.Value) }
            |> ExamIsRunning
        | _ -> ExamIsRunning exam
    | ExamIsRunning exam, HostLeft (_, hostId) ->
        let host = exam.Hosts |> Map.tryFind hostId
        match host with
        | Some h ->
            { exam with
                  Hosts =
                      exam.Hosts
                      |> Map.add hostId (HostInExam.Disconnected h.Value) }
            |> ExamIsRunning
        | _ -> ExamIsRunning exam
    | ExamIsRunning exam, SubmissionCreated (_, submission) ->
        { exam with
              Submissions = submission :: exam.Submissions }
        |> ExamIsRunning
    | ExamIsRunning exam, MessageSent (_, message) ->
        { exam with
              Messages = message :: exam.Messages }
        |> ExamIsRunning
    | ExamIsRunning exam, ExamEnded _ -> ExamIsFinished exam

    | ExamIsFinished exam, StudentLeft (_, studentId) ->
        match exam.Students |> Map.tryFind studentId with
        | Some student -> 
            { exam with
                  Students = exam.Students |> Map.add studentId (StudentInExam.Disconnected student.Value) }
        | _ -> exam
        |> ExamIsFinished
    | ExamIsFinished exam, HostEntered (_, host) ->
        match exam.Hosts |> Map.tryFind host.Id with
        | Some host ->
            { exam with
                  Hosts =
                      exam.Hosts
                      |> Map.add host.Id (HostInExam.Connected host.Value) }
        | _ -> exam
        |> ExamIsFinished
    | ExamIsFinished exam, HostLeft (_, hostId) ->
        let host = exam.Hosts |> Map.tryFind hostId
        match host with
        | Some h ->
            { exam with
                  Hosts =
                      exam.Hosts
                      |> Map.add hostId (HostInExam.Disconnected h.Value) }
        | _ -> exam
        |> ExamIsFinished
    | ExamIsFinished exam, MessageSent (_, message) ->
        { exam with
              Messages = message :: exam.Messages }
        |> ExamIsFinished
    | ExamIsFinished exam, ExamClosed _ -> ExamIsClose(Some exam.Id)
