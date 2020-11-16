namespace Normio.Core.Commands

open System
open Normio.Core

type Command =
    | OpenExam of examId:Guid * title:ExamTitle40 * startTime:DateTime * duration:TimeSpan
    | StartExam of examId:Guid
    | EndExam of examId:Guid
    | CloseExam of examId:Guid

    | AddStudent of examId:Guid * student:Student
    | RemoveStudent of examId:Guid * studentId:Guid
    | AddHost of examId:Guid * host:Host
    | RemoveHost of examId:Guid * hostId:Guid

    | CreateSubmission of examId:Guid * submission:Submission
    | CreateQuestion of examId:Guid * question:Question
    | DeleteQuestion of examId:Guid * questionId:Guid
    
    | SendMessage of examId: Guid * message:Message

    | ChangeTitle of examId:Guid * title:ExamTitle40
