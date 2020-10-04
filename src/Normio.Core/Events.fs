module Normio.Core.Events

open System
open Normio.Core.Domain

type Event =
    | ExamOpened of Guid * string
    | ExamStarted of Guid
    | ExamEnded of Guid
    | ExamClosed of Guid
    | StudentEntered of Guid * Student
    | StudentLeft of Guid * Guid
    | HostEntered of Guid * Host
    | HostLeft of Guid * Guid
    | QuestionCreated of Guid * File
    | QuestionDeleted of Guid * File
    | TitleChanged of Guid * string
