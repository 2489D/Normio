namespace Normio.Core

[<AutoOpen>]
module CommandErrors =
    type CommandError =
        | ExamAlreadyOpened
        | ExamAlreadyStarted
        | ExamAlreadyEnded
        
        | ExamNotOpened
        | ExamNotStarted
        | ExamNotEnded
        
        | NoQuestions
        | NoStudents
        | NoHosts
        
        | CannotFindStudent
        | CannotFindHost
        | CannotFindQuestion
        
        | SubmissionDuplicated
        | IDNotMatched of ctx:string
 
        override this.ToString () =
            match this with
            | ExamAlreadyOpened -> "The exam is already opened"
            | ExamAlreadyStarted -> "The exam is already started"
            | ExamAlreadyEnded -> "The exam is already ended"
            | ExamNotOpened -> "The exam is not opened"
            | ExamNotStarted -> "The exam is not started"
            | ExamNotEnded -> "The exam is not ended"
            | NoQuestions -> "The exam has no questions"
            | NoStudents -> "The exam has no students"
            | NoHosts -> "The exam has no hosts"
            | CannotFindStudent -> "The exam does not have the student"
            | CannotFindHost -> "The exam does not have the host"
            | CannotFindQuestion -> "The exam does not have the question"
            | SubmissionDuplicated -> "The exam already has the submission id"
            | IDNotMatched ctx -> sprintf "Two IDs are not matched: %s" ctx
