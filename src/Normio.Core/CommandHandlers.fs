module Normio.Core.CommandHandlers

open Normio.Core.Domain
open Normio.Core.Events
open Normio.Core.Commands
open Normio.Core.Errors
open Normio.Core.States

let handleOpenExam id title = function
    | ExamIsClose None ->
        [ExamOpened (id, title)] |> Ok
    | _ -> CannotOpenExam "Exam already opened" |> Error

let handleStartExam = function
    | ExamIsWaiting exam ->
        let hasQuestion = exam.Questions |> List.isEmpty
        let hasStudent = exam.Students |> Map.isEmpty
        let hasHost = exam.Hosts |> Map.isEmpty
        if hasQuestion && hasStudent && hasHost
        then [ExamStarted exam.Id] |> Ok
        else CannotStartExam "There is no question" |> Error
    | ExamIsClose _ -> CannotStartExam "Exam not opened" |> Error
    | _ -> CannotStartExam "Exam already started" |> Error

let handleEndExam = function
    | ExamIsRunning exam ->
        [ExamEnded exam.Id] |> Ok
    | ExamIsFinished _ -> CannotEndExam "Exam already ended" |> Error
    | _ -> CannotEndExam "Exam not started" |> Error

let handleCloseExam = function
    | ExamIsFinished exam ->
        [ExamClosed exam.Id] |> Ok
    | _ -> CannotCloseExam "Exam not ended" |> Error

let handleAddStudent student = function
    | ExamIsWaiting exam ->
        [StudentEntered (exam.Id, student)] |> Ok
    | ExamIsClose _ -> CannotAddStudent "Exam not opened" |> Error
    | _ -> CannotAddStudent "Exam already started" |> Error

let handleRemoveStudent studentId = function
    | ExamIsWaiting exam
    | ExamIsFinished exam ->
        if exam.Students |> Map.containsKey studentId
        then [StudentLeft (exam.Id, studentId)] |> Ok
        else CannotRemoveStudent "Student is not in exam" |> Error
    | ExamIsClose _ -> CannotRemoveStudent "Exam not opened" |> Error
    | ExamIsRunning _ -> CannotRemoveStudent "Exam is running" |> Error

let handleAddHost host = function
    | ExamIsWaiting exam
    | ExamIsRunning exam
    | ExamIsFinished exam ->
        [HostEntered (exam.Id, host)] |> Ok
    | _ -> CannotAddHost "Exam not opened" |> Error

let handleRemoveHost hostId = function
    | ExamIsWaiting exam
    | ExamIsRunning exam
    | ExamIsFinished exam ->
        if exam.Hosts |> Map.containsKey hostId
        then [HostLeft (exam.Id, hostId)] |> Ok
        else CannotRemoveHost "Host is not in exam" |> Error
    | _ -> CannotRemoveHost "Exam not opened" |> Error

let handleCreateSubmission submission = function
    | ExamIsRunning exam ->
        [SubmissionCreated (exam.Id, submission)] |> Ok
    | _ -> CannotCreateSubmission "Exam is not running" |> Error

let handleCreateQuestion file = function
    | ExamIsWaiting exam ->
        [QuestionCreated (exam.Id, file)] |> Ok
    | ExamIsClose _ -> CannotCreateQuestion "Exam not opened" |> Error
    | _ -> CannotCreateQuestion "Exam already started" |> Error

let handleDeleteQuestion questionId = function
    | ExamIsWaiting exam ->
        if exam.Questions |> List.exists (fun questionId' -> questionId' = questionId)
        then [QuestionDeleted (exam.Id, questionId)] |> Ok
        else CannotDeleteQuestion "There is no such question file" |> Error
    | ExamIsClose _ -> CannotDeleteQuestion "Exam not opened" |> Error
    | _ -> CannotDeleteQuestion "Exam already started" |> Error

let handleChangeTitle title = function
    | ExamIsWaiting exam ->
        [TitleChanged (exam.Id, title)] |> Ok
    | ExamIsClose _ -> CannotChangeTitle "Exam not opened" |> Error
    | _ -> CannotChangeTitle "Exam already started" |> Error


/// Produces a list of events as a consequence of a command into a state
let execute state = function
    | OpenExam (id, title) -> handleOpenExam id title state
    | StartExam _ -> handleStartExam state
    | EndExam _ -> handleEndExam state
    | CloseExam _ -> handleCloseExam state
    | AddStudent (_, student) -> handleAddStudent student state
    | RemoveStudent (_, studentId) -> handleRemoveStudent studentId state
    | AddHost (_, host) -> handleAddHost host state
    | RemoveHost (_, hostId) -> handleRemoveHost hostId state
    | CreateSubmission (_, submission) -> handleCreateSubmission submission state
    | CreateQuestion (_, questionId) -> handleCreateQuestion questionId state
    | DeleteQuestion (_, questionId) -> handleDeleteQuestion questionId state
    | ChangeTitle (_, title) -> handleChangeTitle title state


let evolve state command =
    match execute state command with
    | Ok newEvents ->
        let newState = List.fold apply state newEvents 
        (newState, newEvents) |> Ok
    | Error err -> Error err

