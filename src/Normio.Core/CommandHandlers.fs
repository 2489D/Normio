module Normio.Core.CommandHandlers

open Normio.Core
open Normio.Core.Commands
open Normio.Core.States

let handleOpenExam id title = function
    | ExamIsClose None ->
        [ExamOpened (id, title)] |> Ok
    | _ -> ExamAlreadyOpened |> Error

let handleStartExam = function
    | ExamIsWaiting exam ->
        let noQuestions = exam.Questions |> List.isEmpty
        let noStudents = exam.Students |> Map.isEmpty
        let noHosts = exam.Hosts |> Map.isEmpty
        match (noQuestions, noStudents, noHosts) with
        | true, _, _ -> Error NoQuestions
        | _, true, _ -> Error NoStudents
        | _, _, true -> Error NoHosts
        | _ -> Ok [ExamStarted exam.Id]
    | ExamIsClose _ -> ExamNotOpened |> Error
    | _ -> ExamAlreadyStarted |> Error

let handleEndExam = function
    | ExamIsRunning exam ->
        [ExamEnded exam.Id] |> Ok
    | ExamIsFinished _ -> Error ExamAlreadyEnded
    | _ -> Error ExamNotStarted

let handleCloseExam = function
    | ExamIsFinished exam ->
        [ExamClosed exam.Id] |> Ok
    | _ -> Error ExamNotEnded

let handleAddStudent student = function
    | ExamIsWaiting exam ->
        [StudentEntered (exam.Id, student)] |> Ok
    | ExamIsClose _ -> ExamNotOpened |> Error
    | _ -> ExamAlreadyStarted |> Error

let handleRemoveStudent studentId = function
    | ExamIsWaiting exam
    | ExamIsFinished exam ->
        if exam.Students |> Map.containsKey studentId
        then [StudentLeft (exam.Id, studentId)] |> Ok
        else CannotFindStudent |> Error
    | ExamIsClose _ -> ExamNotOpened |> Error
    | ExamIsRunning _ -> ExamAlreadyStarted |> Error

let handleAddHost host = function
    | ExamIsWaiting exam
    | ExamIsRunning exam
    | ExamIsFinished exam ->
        [HostEntered (exam.Id, host)] |> Ok
    | _ -> ExamNotOpened |> Error

let handleRemoveHost hostId = function
    | ExamIsWaiting exam
    | ExamIsRunning exam
    | ExamIsFinished exam ->
        if exam.Hosts |> Map.containsKey hostId
        then [HostLeft (exam.Id, hostId)] |> Ok
        else CannotFindHost |> Error
    | _ -> ExamNotOpened |> Error

let handleCreateSubmission (submission: Submission) = function
    | ExamIsRunning exam ->
        let isSubmissionDuplicated = exam.Submissions |> List.exists (fun subm -> subm.Id = submission.Id)
        let areIDsDifferent = submission.ExamId <> exam.Id
        let doesNotExamHasTheStudent = exam.Students |> Map.containsKey submission.StudentId |> not
        match (isSubmissionDuplicated, areIDsDifferent, doesNotExamHasTheStudent) with
        | true, _, _ -> SubmissionDuplicated |> Error
        | _, true, _ -> IDNotMatched "The exam id of the submission is different from the exam id provided" |> Error
        | _, _, true -> IDNotMatched "The exam does not have the student id of the submission" |> Error
        | _ -> [SubmissionCreated (exam.Id, submission)] |> Ok
    | _ -> ExamNotStarted |> Error

let handleCreateQuestion (question: Question) = function
    | ExamIsWaiting exam ->
        let isQuestionDuplicated = exam.Questions |> List.exists (fun que -> que.Id = question.Id)
        let areIDsDifferent = question.ExamId <> exam.Id
        let doesNotExamHasTheHost = exam.Hosts |> Map.containsKey question.HostId |> not
        match (isQuestionDuplicated, areIDsDifferent, doesNotExamHasTheHost) with
        | true, _, _ -> QuestionDuplicated |> Error
        | _, true, _ -> IDNotMatched "The exam id of the question is different from the exam id provided" |> Error
        | _, _, true -> IDNotMatched "The exam does not have the host id of the question" |> Error
        | _ -> [QuestionCreated (exam.Id, question)] |> Ok
    | ExamIsClose _ -> ExamNotOpened |> Error
    | _ -> ExamAlreadyStarted |> Error

let handleDeleteQuestion questionId = function
    | ExamIsWaiting exam ->
        if exam.Questions |> List.exists (fun question -> question.Id = questionId)
        then [QuestionDeleted (exam.Id, questionId)] |> Ok
        else CannotFindQuestion |> Error
    | ExamIsClose _ -> ExamNotOpened |> Error
    | _ -> ExamAlreadyStarted |> Error

let handleSendMessage message = function
    | ExamIsWaiting exam
    | ExamIsRunning exam
    | ExamIsFinished exam -> [MessageSent (exam.Id, message)] |> Ok
    | ExamIsClose _ -> ExamNotOpened |> Error

let handleChangeTitle title = function
    | ExamIsWaiting exam ->
        [TitleChanged (exam.Id, title)] |> Ok
    | ExamIsClose _ -> ExamNotOpened |> Error
    | _ -> ExamAlreadyStarted |> Error


/// Produces a list of event results as a consequence of a command into a state
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
    | SendMessage (_, message) -> handleSendMessage message state
    | ChangeTitle (_, title) -> handleChangeTitle title state


let evolve state command =
    match execute state command with
    | Ok newEvents ->
        let newState = List.fold apply state newEvents 
        (newState, newEvents) |> Ok
    | Error err -> Error err

