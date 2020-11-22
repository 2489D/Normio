namespace Normio.Core.Domain

open System

type Undefined = exn

[<AutoOpen>]
module Exams =

    [<AutoOpen>]
    module Users =
        type EnrolledHost =
            | Connected of AuthorisedUser
            | Disconnected of AuthorisedUser

            member this.Id =
                match this with
                | Connected authedUser -> authedUser.Id
                | Disconnected authedUser -> authedUser.Id

        type EnrolledParticipant =
            | Connected of AuthorisedUser
            | Disconnected of AuthorisedUser

            member this.Id =
                match this with
                | Connected authedUser -> authedUser.Id
                | Disconnected authedUser -> authedUser.Id

    // TODO : scoring policy
    type Exam =
        { Id : Guid
          Title : ExamTitle40
          Users : ExamUsers
          Questions : ExamQuestions
          Messages : ExamMessages
          OperationPolicy : ExamOperationPolicy
          CreatedDateTime : DateTime
          ModifiedDateTimes : DateTime list }
    
    and ExamUsers =
        { ExamUserPolicy : Set<ExamUserPolicy>
          Hosts : ExamHosts
          Participants: Map<Guid, EnrolledParticipant> }
    
    and ExamUserPolicy =
        | AllowLateEntrance of lateLimit : TimeSpan
    
    and ExamHosts =
        { Admins: EnrolledHost * EnrolledHost list
          Managers: EnrolledHost list }

    and ExamQuestions =
        { Id : Guid
          SubmissionPolicy : SubmissionPolicy
          Contents : QuestionContent list
          Submissions : Submission list }
    
    and SubmissionPolicy =
        | AllowLateSubmission of lateLimit : TimeSpan option
        | AllowMultipleSubmissions of countLimit : uint option
    
    and QuestionContent =
        | File of FileQuestion
        | Text of TextQuestion
        | SingleChoice of SingleChoiceQuestion
        | MultipleChoice of MultipleChoiceQuestion
    
    and FileQuestion =
        { Id : Guid
          ExamId : Guid
          HostId : Guid
          Title : string
          Question : Undefined
          Description : string option
          CreatedDateTime : DateTime
          Submissions : Submission list }
    
    and TextQuestion =
        { Id : Guid
          ExamId : Guid
          HostId : Guid
          Title : string
          Question : Undefined
          Description : string option
          CreatedDateTime : DateTime
          Submissions : Submission list }
        
    // TODO : how to design Choice type in a good way?
    // Requirements:
    // 1. should represent varying length of choices
    // 2. should prevent indices of question and submission from conflicting at run time
    and Choice = uint

    and SingleChoiceQuestion =
        { Id : Guid
          ExamId : Guid
          HostId : Guid
          Title : string
          Question : (uint * string) list
          Description : string option
          CreatedDateTime : DateTime
          Submissions : SingleChoiceSubmission list }

    and MultipleChoiceQuestion =
        { Id : Guid
          ExamId : Guid
          HostId : Guid
          Title : string
          Question : (uint * string) list
          Description : string option
          CreatedDateTime : DateTime
          Submissions : MultipleChoiceSubmission list }

    and Submission =
        | FileSubmission of FileSubmission
        | TextSubmission of TextSubmission
        | SingleChoiceSubmission of SingleChoiceSubmission
        | MultipleChoiceSubmission of MultipleChoiceSubmission
    
    and FileSubmission =
        { Id : Guid
          ExamId : Guid
          CreatedDateTime : DateTime
          Content : Undefined }
    
    and TextSubmission =
        { Id : Guid
          ExamId : Guid
          CreatedDateTime : DateTime
          Content : string }

    and SingleChoiceSubmission =
        { Id : Guid
          ExamId : Guid
          CreatedDateTime : DateTime
          Content : Choice }

    and MultipleChoiceSubmission =
        { Id : Guid
          ExamId : Guid
          CreatedDateTime : DateTime
          Content : Choice list }

    and ExamSubmissionContent =
        | File
        | Text
        | Selections
        | SourceCode

    and ExamMessages =
        { MessagePolicy : Set<MessagePolicy>
          Messages : Map<Guid, Message> }

    and MessagePolicy =
        | AllowPublicMessageOfParticipant
    
    and Message =
        | MessageFromParticipantToHosts of MessageDetail
        | MessageFromParticipantToEveryone of MessageDetail
        | MessageFromHostToEveryone of MessageDetail
        | MessageFromHostToHosts of MessageDetail

    and MessageDetail =
        { Id : Guid
          ExamId : Guid
          From : Guid
          To : Guid list
          CreatedDateTime : DateTime
          Content : string }

    and Notice =
        { Id : Guid
          Author : User
          Content : string }

    and ExamOperationPolicy =
        | ReservedStart of startDateTime : DateTime
        | ReservedStartAndEnd of startDateTime : DateTime * duration : TimeSpan
        | FixedDuration of duration : TimeSpan
        | Manually
