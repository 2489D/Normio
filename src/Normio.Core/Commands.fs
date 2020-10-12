module Normio.Core.Commands

open System
open Normio.Core.Domain

type Command =
    | OpenExam of Guid * ExamTitle40
    | StartExam of Guid
    | EndExam of Guid
    | CloseExam of Guid
    | AddStudent of Guid * Student
    | RemoveStudent of Guid * Guid
    | AddHost of Guid * Host
    | RemoveHost of Guid * Guid
    | CreateSubmission of Guid * Submission
    | CreateQuestion of Guid * File
    | DeleteQuestion of Guid * File
    | ChangeTitle of Guid * ExamTitle40
