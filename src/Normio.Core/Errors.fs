module Normio.Errors

type Error =
    | ExamAlreadyStarted
    | CannotEndExam
    | CannotCloseRoom

module Error =
    let toString = function
        | ExamAlreadyStarted -> "The exam already started."
        | CannotEndExam -> "Cannot end the exam."
        | CannotCloseRoom -> "Cannot close the room."

