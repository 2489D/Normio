module Normio.Web.Requests

open System
open System.Runtime.Serialization
open Suave.Json

[<DataContract>]
type OpenExam = {
    [<field: DataMember(Name="title")>]
    Title: string
}

[<DataContract>]
type StartExam = {
    [<field: DataMember(Name="examId")>]
    ExamId: Guid
}

[<DataContract>]
type EndExam = {
    [<field: DataMember(Name="examId")>]
    ExamId: Guid
}

[<DataContract>]
type CloseExam = {
    [<field: DataMember(Name="examId")>]
    ExamId: Guid
}

[<DataContract>]
type GetStudents = {
    [<field: DataMember(Name="examId")>]
    ExamId: Guid
}

[<DataContract>]
type Student = {
    [<field: DataMember(Name="studentId")>]
    StudentId: Guid
    [<field: DataMember(Name="name")>]
    Name: string
}

[<DataContract>]
type AddStudent = {
    [<field: DataMember(Name="name")>]
    Name: string
}

[<DataContract>]
type RemoveStudent = {
    [<field: DataMember(Name="studentId")>]
    StudentId: Guid
}

[<DataContract>]
type AddHost = {
    [<field: DataMember(Name="name")>]
    Name: string
}

[<DataContract>]
type RemoveHost = {
    [<field: DataMember(Name="hostId")>]
    HostId: Guid
}

[<DataContract>]
type CreateQuestion = {
    [<field: DataMember(Name="examId")>]
    ExamId: Guid
}

[<DataContract>]
type DeleteQuestion = {
    [<field: DataMember(Name="examId")>]
    ExamId: Guid
}