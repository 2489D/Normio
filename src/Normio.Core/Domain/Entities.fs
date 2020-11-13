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

    [<CustomEquality; NoComparison>]
    [<JsonFSharpConverter>]
    type Question = {
        [<JsonPropertyName("id")>]
        Id: Guid
        [<JsonPropertyName("examId")>]
        ExamId: Guid
        [<JsonPropertyName("hostId")>]
        HostId: Guid
        [<JsonPropertyName("timestamp")>]
        TimeStamp: DateTime
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
        [<JsonPropertyName("timestamp")>]
        TimeStamp: DateTime
    } with
        override this.Equals(other) =
            match other with
            | :? Submission as sbms -> this.Id = sbms.Id
            | _ -> false
        override this.GetHashCode() =
            hash this.Id

    // TODO : Make Question Entity
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
        [<JsonPropertyName("students")>]
        Students: Map<Guid, Student>
        [<JsonPropertyName("hosts")>]
        Hosts: Map<Guid, Host>
    } with
        override this.Equals(other) =
            match other with
            | :? Exam as e -> this.Id = e.Id
            | _ -> false
        override this.GetHashCode() =
            hash this.Id
            
