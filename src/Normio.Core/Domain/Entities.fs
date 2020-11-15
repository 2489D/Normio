namespace Normio.Core

open System
open System.Text.Json.Serialization

[<AutoOpen>]
module Entities = 
    [<CustomEquality; NoComparison>]
    [<JsonFSharpConverter>]
    type Student = {
        [<JsonPropertyName("id")>]
        Id: Guid
        [<JsonPropertyName("name")>]
        Name: UserName40
    } with
        override this.Equals(other) =
            match other with
            | :? Student as s -> this.Id = s.Id
            | _ -> false
        override this.GetHashCode() =
            hash this.Id
        
    [<CustomEquality; NoComparison>]
    [<JsonFSharpConverter>]
    type Host = {
        [<JsonPropertyName("id")>]
        Id: Guid
        [<JsonPropertyName("name")>]
        Name: UserName40
    } with
        override this.Equals(other) =
            match other with
            | :? Host as h -> this.Id = h.Id
            | _ -> false
        override this.GetHashCode() =
            hash this.Id

    // TODO: add auto processing function
    type MessageFromStudentToHost =
        { Id: Guid
          ExamId: Guid
          SenderStudent: Guid
          ReceiverHost: Guid
          CreatedDateTime: DateTime
          Content: MessageContent }
        
    type MessageFromHostToStudents =
        { Id: Guid
          ExamId: Guid
          SenderHost: Guid
          ReceiverStudents: Guid list
          CreatedDateTime: DateTime
          Content: MessageContent }
 
    type MessageFromHostToHosts =
        { Id: Guid
          ExamId: Guid
          SenderHost: Guid
          ReceiverHosts: Guid list
          CreatedDateTime: DateTime
          Content: MessageContent }

    type Notice =
        { Id: Guid
          ExamId: Guid
          SenderHost: Guid
          CreatedDateTime: DateTime
          Content: MessageContent }
    
    [<CustomEquality; NoComparison>]
    type Message =
        | MessageFromStudentToHost of MessageFromStudentToHost
        | MessageFromHostToStudents of MessageFromHostToStudents
        | MessageFromHostToHosts of MessageFromHostToHosts
        | Notice of Notice
        with
            member this.Id =
                match this with
                | MessageFromStudentToHost msg -> msg.Id
                | MessageFromHostToStudents msg -> msg.Id
                | MessageFromHostToHosts msg -> msg.Id
                | Notice msg -> msg.Id
 
            member this.ExamId =
                match this with
                | MessageFromStudentToHost msg -> msg.ExamId
                | MessageFromHostToStudents msg -> msg.ExamId
                | MessageFromHostToHosts msg -> msg.ExamId
                | Notice msg -> msg.ExamId
            
            member this.Sender =
                match this with
                | MessageFromStudentToHost msg -> msg.SenderStudent
                | MessageFromHostToStudents msg -> msg.SenderHost
                | MessageFromHostToHosts msg -> msg.SenderHost
                | Notice msg -> msg.SenderHost

            member this.Content =
                match this with
                | MessageFromStudentToHost msg -> msg.Content
                | MessageFromHostToStudents msg -> msg.Content
                | MessageFromHostToHosts msg -> msg.Content
                | Notice msg -> msg.Content

            override this.Equals(other) =
                match other with
                | :? Message as otherMsg -> 
                    match this with
                    | MessageFromStudentToHost msg -> msg.Id = otherMsg.Id
                    | MessageFromHostToStudents msg -> msg.Id = otherMsg.Id
                    | MessageFromHostToHosts msg -> msg.Id = otherMsg.Id
                    | Notice msg -> msg.Id = otherMsg.Id
                | _ -> false
            
            override this.GetHashCode() =
                hash this.Id

    [<CustomEquality; NoComparison>]
    [<JsonFSharpConverter>]
    type Question = {
        [<JsonPropertyName("id")>]
        Id: Guid
        [<JsonPropertyName("examId")>]
        ExamId: Guid
        [<JsonPropertyName("hostId")>]
        HostId: Guid
        [<JsonPropertyName("title")>]
        Title: string
        [<JsonPropertyName("timestamp")>]
        CreatedDateTime: DateTime
        [<JsonPropertyName("description")>]
        Description: string option
    } with
        override this.Equals(other) =
            match other with
            | :? Question as sbms -> this.Id = sbms.Id
            | _ -> false
        override this.GetHashCode() =
            hash this.Id

    [<CustomEquality; NoComparison>]
    [<JsonFSharpConverter>]
    type Submission = {
        [<JsonPropertyName("id")>]
        Id: Guid
        [<JsonPropertyName("examId")>]
        ExamId: Guid
        [<JsonPropertyName("studentId")>]
        StudentId: Guid
        [<JsonPropertyName("title")>]
        Title: string
        [<JsonPropertyName("timestamp")>]
        CreatedDateTime: DateTime
        [<JsonPropertyName("description")>]
        Description: string option
    } with
        override this.Equals(other) =
            match other with
            | :? Submission as sbms -> this.Id = sbms.Id
            | _ -> false
        override this.GetHashCode() =
            hash this.Id

    [<CustomEquality; NoComparison>]
    [<JsonFSharpConverter>]
    type Exam = {
        [<JsonPropertyName("id")>]
        Id: Guid
        [<JsonPropertyName("title")>]
        Title: ExamTitle40
        [<JsonPropertyName("questions")>]
        Questions: Question list
        [<JsonPropertyName("submissions")>]
        Submissions: Submission list
        [<JsonPropertyName("messages")>]
        Messages: Message list
        [<JsonPropertyName("students")>]
        Students: Map<Guid, Student>
        [<JsonPropertyName("hosts")>]
        Hosts: Map<Guid, Host>
        [<JsonPropertyName("createdDateTime")>]
        CreatedDateTime: DateTime
        [<JsonPropertyName("startDateTime")>]
        StartDateTime: DateTime option
        [<JsonPropertyName("duration")>]
        Duration: TimeSpan option
    } with
        override this.Equals(other) =
            match other with
            | :? Exam as e -> this.Id = e.Id
            | _ -> false
        override this.GetHashCode() =
            hash this.Id
        
        static member Initial id title =
            { Id = id
              Title = title
              Questions = []
              Submissions = []
              Messages = []
              Students = Map.empty
              Hosts = Map.empty
              CreatedDateTime = DateTime.Now
              StartDateTime = None
              Duration = None }
            
            
