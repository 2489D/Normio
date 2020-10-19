namespace Normio.Core.Domain

open System
open System.Text.Json.Serialization

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
type File = {
    [<JsonPropertyName("id")>]
    Id: Guid
    [<JsonPropertyName("name")>]
    FileName: FileString200
} with
    override this.Equals(other) =
        match other with
        | :? File as f -> this.Id = f.Id
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
    Student: Student
    File: File
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
    [<JsonPropertyName("question")>]
    Questions: File list
    Submissions: Submission list
    Students: Map<Guid, Student>
    Hosts: Map<Guid, Host>
} with
    override this.Equals(other) =
        match other with
        | :? Exam as e -> this.Id = e.Id
        | _ -> false
    override this.GetHashCode() =
        hash this.Id
