namespace Normio.Core.Domain

open System
open System.Xml.Schema

[<CustomEquality; NoComparison>]
type Student = {
    Id: Guid
    Name: UserName40
} with
    override this.Equals(other) =
        match other with
        | :? Student as s -> this.Id = s.Id
        | _ -> false
    override this.GetHashCode() =
        hash this.Id

[<CustomEquality; NoComparison>]
type Host = {
    Id: Guid
    Name: UserName40
} with
    override this.Equals(other) =
        match other with
        | :? Host as h -> this.Id = h.Id
        | _ -> false
    override this.GetHashCode() =
        hash this.Id

[<CustomEquality; NoComparison>]
type File = {
    Id: Guid
    FileName: FileString200
} with
    override this.Equals(other) =
        match other with
        | :? File as f -> this.Id = f.Id
        | _ -> false
    override this.GetHashCode() =
        hash this.Id

[<CustomEquality; NoComparison>]
type Submission = {
    Id: Guid
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
type Exam = {
    Id: Guid
    Title: ExamTitle40
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
