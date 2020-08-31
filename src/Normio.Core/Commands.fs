module Normio.Commands

open System

type Command =
    | StartExam of Guid
    | EndExam of Guid
    | CloseRoom of Guid

