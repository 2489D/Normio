namespace Normio.Storage.EventStore

open System
open Normio.Core.Events
open Normio.Core.States
module Helper =
    let getExamIdFromEvent = function
        | ExamOpened (id, _) -> id
        | ExamStarted id -> id
        | ExamEnded id -> id
        | ExamClosed id -> id
        | StudentEntered (id, _) -> id
        | StudentLeft (id, _) -> id
        | HostEntered (id, _) -> id
        | HostLeft (id, _) -> id
        | SubmissionCreated (id, _) -> id
        | QuestionCreated (id, _) -> id
        | QuestionDeleted (id, _) -> id
        | TitleChanged (id, _) -> id

type EventStore = {
    GetState: Guid -> Async<State>
    SaveEvents: Event list -> Async<unit>
}
