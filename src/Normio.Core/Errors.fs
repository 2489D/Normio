module Normio.Core.Errors

type Error =
    | ExamAlreadyStarted
    | CannotEndExam
    | CannotCloseRoom
    | RoomAlreadyOpened

module Error =
    let toString = function
        | ExamAlreadyStarted -> "The exam already started."
        | CannotEndExam -> "Cannot end the exam."
        | CannotCloseRoom -> "Cannot close the room."
        | RoomAlreadyOpened -> "The room has been opened already"

