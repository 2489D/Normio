namespace Normio.Core.Events

open System
open Normio.Core.Domain

type Event =
    | ExamOpened of examId:Guid * title:ExamTitle40
    | ExamStarted of examId:Guid
    | ExamEnded of examId:Guid
    | ExamClosed of examId:Guid
    | StudentEntered of examId:Guid * student:Student
    | StudentLeft of examId:Guid * studentId:Guid
    | HostEntered of examId:Guid * host:Host
    | HostLeft of examId:Guid * hostId:Guid
    | SubmissionCreated of examId:Guid * submission:Submission
    | QuestionCreated of examId:Guid * file:File
    | QuestionDeleted of examId:Guid * fileId:Guid
    | TitleChanged of examId:Guid * title:ExamTitle40
    
