module Normio.Errors

type Error =
  | ExamAlreadyStarted
  | CannotEndExam

module Error =
  let toString = function
    | ExamAlreadyStarted -> "Exam already started."
    | CannotEndExam -> "Cannot end exam."