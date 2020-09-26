module Normio.CommandHandlers

open Events
open Commands
open Errors
open States

let handleOpenExam id title = function
    | ExamIsClose None ->
        [ExamOpened (id, title)] |> Ok
    | _ -> Error "Unable to open an exam"

let handleStartExam id = function
    | ExamIsWaiting exam ->
        [ExamStarted exam.Id] |> Ok
    | _ -> Error "Unable to start the exam"

let handleEndExam id = function
    | ExamIsRunning exam ->
        [ExamEnded exam.Id] |> Ok
    | _ -> Error "Unable to end the exam"

let handleCloseExam id = function
    | ExamIsFinished exam ->
        [ExamClosed exam.Id] |> Ok
    | _ -> Error "Unable to close the exam"

let handleAddStudent id student = function
    | ExamIsWaiting exam ->
        [StudentEntered (exam.Id, student)] |> Ok
    | _ -> Error "Unable to add the student into the exam"

let handleRemoveStudent id student = function
    | ExamIsWaiting exam ->
        [StudentLeft (exam.Id, student)] |> Ok
    | _ -> Error "Unable to remove the student at the exam"

let handleAddHost id host = function
    | ExamIsWaiting exam ->
        [HostEntered (exam.Id, host)] |> Ok
    | _ -> Error "Unable to add the host into the exam"

let handleRemoveHost id host = function
    | ExamIsWaiting exam ->
        [HostLeft (exam.Id, host)] |> Ok
    | _ -> Error "Unable to remove the host into the exam"

let handleCreateQuestion id file = function
    | ExamIsWaiting exam ->
        [QuestionCreated (exam.Id, file)] |> Ok
    | _ -> Error "Unable to create the question on the exam"

let handleDeleteQuestion id file = function
    | ExamIsWaiting exam ->
        [QuestionDeleted (exam.Id, file)] |> Ok
    | _ -> Error "Unable to remove the question on the exam"

let handleChangeTitle id title = function
    | ExamIsWaiting exam
    | ExamIsRunning exam ->
        [TitleChanged (exam.Id, title)] |> Ok
    | _ -> Error "Unable to change the title of the exam"


/// Produces a list of events as a consequence of a command into a state
let execute state = function
    | OpenExam (id, title) -> handleOpenExam id title state
    | StartExam id -> handleStartExam id state
    | EndExam id -> handleEndExam id state
    | CloseExam id -> handleCloseExam id state
    | AddStudent (id, student) -> handleAddStudent id student state
    | RemoveStudent (id ,student) -> handleRemoveStudent id student state
    | AddHost (id, host) -> handleAddHost id host state
    | RemoveHost (id, host) -> handleRemoveHost id host state
    | CreateQuestion (id, file) -> handleCreateQuestion id file state
    | DeleteQuestion (id, file) -> handleDeleteQuestion id file state
    | ChangeTitle (id, title) -> handleChangeTitle id title state


let evolve state command =
    match execute state command with
    | Ok newEvents ->
        let newState = List.fold apply state newEvents 
        (newState, newEvents) |> Ok
    | Error err -> Error err

