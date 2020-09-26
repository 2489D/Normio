module Normio.Events

open System
open Domain

type Event =
    | ExamOpened of Guid * string
    | ExamStarted of Guid
    | ExamEnded of Guid
    | ExamClosed of Guid
    | StudentEntered of Guid * Student
    | StudentLeft of Guid * Student
    | HostEntered of Guid * Host
    | HostLeft of Guid * Host
    | QuestionCreated of Guid * File
    | QuestionDeleted of Guid * File
    | TitleChanged of Guid * string
