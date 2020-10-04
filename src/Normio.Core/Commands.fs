module Normio.Core.Commands

open System
open Normio.Core.Domain

type Command =
    | OpenExam of Guid * string
    | StartExam of Guid
    | EndExam of Guid
    | CloseExam of Guid
    | AddStudent of Guid * Student
    | RemoveStudent of Guid * Student
    | AddHost of Guid * Host
    | RemoveHost of Guid * Host
    | CreateQuestion of Guid * File
    | DeleteQuestion of Guid * File
    | ChangeTitle of Guid * string
