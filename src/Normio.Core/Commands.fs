namespace Normio.Core.Commands

open System
open Normio.Core

type Command =
    | OpenExam of examId:Guid * title:ExamTitle40
    | StartExam of examId:Guid
    | EndExam of examId:Guid
    | CloseExam of examId:Guid
    | AddStudent of examId:Guid * student:Student
    | RemoveStudent of examId:Guid * studentId:Guid
    | AddHost of examId:Guid * host:Host
    | RemoveHost of examId:Guid * hostId:Guid
    | CreateSubmission of examId:Guid * submission:Submission
    | CreateQuestion of examId:Guid * question:File
    | DeleteQuestion of examId:Guid * questionId:Guid
    | ChangeTitle of examId:Guid * title:ExamTitle40
