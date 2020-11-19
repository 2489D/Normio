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
    | LetStudentIn of examId:Guid * studentId:Guid
    | LetStudentOut of examId:Guid * studentId:Guid

    | AddHost of examId:Guid * host:Host
    | RemoveHost of examId:Guid * hostId:Guid
    | LetHostIn of examId:Guid * hostId:Guid
    | LetHostOut of examId:Guid * hostId:Guid

    | CreateSubmission of examId:Guid * submission:Submission
    | CreateQuestion of examId:Guid * question:Question
    | DeleteQuestion of examId:Guid * questionId:Guid
    
    | SendMessage of examId: Guid * message:Message

    | ChangeTitle of examId:Guid * title:ExamTitle40
    with
        member this.ExamId =
            match this with
            | OpenExam (examId, _, _, _) -> examId
            | StartExam examId -> examId
            | EndExam examId -> examId
            | CloseExam examId -> examId

            | AddStudent (examId, _) -> examId
            | RemoveStudent (examId, _) -> examId
            | LetStudentIn (examId, _) -> examId
            | LetStudentOut (examId, _) -> examId
            | AddHost (examId, _) -> examId
            | RemoveHost (examId, _) -> examId
            | LetHostIn (examId, _) -> examId
            | LetHostOut (examId, _) -> examId

            | CreateSubmission (examId, _) -> examId
            | CreateQuestion (examId, _) -> examId
            | DeleteQuestion (examId, _) -> examId
            
            | SendMessage (examId, _) -> examId

            | ChangeTitle (examId, _) -> examId
