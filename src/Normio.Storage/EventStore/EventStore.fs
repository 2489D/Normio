namespace Normio.Storage.EventStore

open System
open Normio.Core.Events
open Normio.Core.States

[<AutoOpen>]
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

type IEventStore =
    abstract GetState: examId:Guid -> Async<State>
    abstract SaveEvents: event:Event list -> Async<unit>

